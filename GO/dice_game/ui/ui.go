package ui

import (
	"bufio"
	"dice_game/models"
	"fmt"
	"os"
	"os/exec"
	"runtime"
	"strconv"
	"strings"
	"unicode"
)

type UI interface {
	ClearScr()
	PauseForUser()
	GoodbyeMsg()
	PlayerNamePrompt(p, pCount int) string
	ScoreLimitPrompt() int
	GameMenuPrompt() int
	GameStartPrompt(scoreLimit int)
	RollPrompt()
	PlayersCountPrompt() int
	DisplayInfo(participants []models.Participant)
	DisplayRoundOutcome(participants []models.Participant, rollRes []models.RollResult, winnerName string)
	AnnounceWinner(winner models.Participant)
}

type consoleUI struct {
	reader *bufio.Reader
}

func New() UI {
	return &consoleUI{
		reader: bufio.NewReader(os.Stdin),
	}
}

func (u *consoleUI) ClearScr() {
	var cmd *exec.Cmd
	if runtime.GOOS == "windows" {
		cmd = exec.Command("cmd", "/c", "cls")
	} else {
		cmd = exec.Command("clear")
	}
	cmd.Stdout = os.Stdout
	cmd.Run()
}

func (u *consoleUI) getUserInput(prompt string) (string, error) {
	fmt.Print(prompt)
	input, err := u.reader.ReadString('\n')

	if err != nil {
		return "", err
	}

	return strings.TrimSpace(input), nil
}

func (u *consoleUI) PauseForUser() {
	fmt.Print("\nPress 'Enter' to continue...")
	u.reader.ReadString('\n')
}

func (u *consoleUI) handleInputErr(message string) {
	fmt.Println(message)
	u.PauseForUser()
}

func (u *consoleUI) welcomeMsg() {
	fmt.Printf("------- Welcome to the Dice Game! -------\n\n")
}

func (u *consoleUI) GoodbyeMsg() {
	fmt.Println("\nCome to play again...\nGoodbye!")
}

func (u *consoleUI) choiceMsg() {
	fmt.Printf("------- Please make a choice -------\n\n")
}

func (u *consoleUI) PlayerNamePrompt(p, pCount int) string {
	for {
		u.ClearScr()

		prompt := "Enter your name: "
		if pCount > 1 {
			prompt = fmt.Sprintf("Enter name for Player %d: ", p)
		}

		name, err := u.getUserInput(prompt)
		if err != nil {
			u.handleInputErr("An error occurred, please try again.")
			continue
		}

		if name == "" || len(name) < 4 {
			u.handleInputErr("Name must be at least 4 characters long. Please try again.")
			continue
		}

		runes := []rune(name)
		runes[0] = unicode.ToUpper(runes[0])
		return string(runes)
	}
}

func (u *consoleUI) ScoreLimitPrompt() int {
	for {
		u.ClearScr()

		input, err := u.getUserInput("Enter the score limit (3 - 9): ")
		if err != nil {
			u.handleInputErr("An error occurred, please try again.")
			continue
		}

		limit, err := strconv.Atoi(input)
		if err != nil {
			u.handleInputErr("Invalid input. Please enter a whole number.")
			continue
		}

		if limit < 3 || limit > 9 {
			u.handleInputErr("The limit must be between 3 and 9. Please try again.")
			continue
		}

		return limit
	}
}

func (u *consoleUI) GameMenuPrompt() int {
	for {
		u.ClearScr()
		u.welcomeMsg()
		u.choiceMsg()

		fmt.Println("1. Play a game")
		fmt.Println("0. Exit")

		input, err := u.getUserInput("\nYour choice: ")
		if err != nil {
			u.handleInputErr("An error occurred, please try again.")
			continue
		}

		playerChoice, err := strconv.Atoi(input)
		if err != nil {
			u.handleInputErr("Invalid input. Please enter a whole number.")
			continue
		}

		if playerChoice != 1 && playerChoice != 0 {
			u.handleInputErr("Please enter a valid menu value.")
			continue
		}

		return playerChoice
	}
}

func (u *consoleUI) GameStartPrompt(scoreLimit int) {
	fmt.Printf("------- Starting the game: First to collect %d points wins! -------\n\n", scoreLimit)
	u.PauseForUser()
}

func (u *consoleUI) RollPrompt() {
	fmt.Println(">>> Roll your dices! <<<")
	u.PauseForUser()
}

func (u *consoleUI) PlayersCountPrompt() int {
	for {
		u.ClearScr()

		input, err := u.getUserInput("Enter the number of players (1 - 2): ")
		if err != nil {
			u.handleInputErr("An error occurred, please try again.")
			continue
		}

		count, err := strconv.Atoi(input)
		if err != nil {
			u.handleInputErr("Invalid input. Please enter a whole number.")
			continue
		}

		if count < 1 || count > 2 {
			u.handleInputErr("The number of players must be between 1 and 2. Please try again.")
			continue
		}

		return count
	}
}

func (u *consoleUI) DisplayInfo(participants []models.Participant) {
	if len(participants) == 0 {
		return
	}
	for _, p := range participants {
		fmt.Printf(">>>\t%s - scores: %d\t||\tScore limit: %d\t<<<\n", p.GetName(), p.GetScore(), p.GetLimit())
	}
	fmt.Println()
}

func (u *consoleUI) announceRoundWinner(name string) {
	fmt.Printf("\n>>> %s wins the round! <<<\n", name)
}

func (u *consoleUI) DisplayRoundOutcome(participants []models.Participant, rollRes []models.RollResult, winnerName string) {
	u.ClearScr()
	u.DisplayInfo(participants)

	for _, r := range rollRes {
		fmt.Printf("%s rolled a %d\n", r.Participant.GetName(), r.RollValue)
	}

	if winnerName != "" {
		u.announceRoundWinner(winnerName)
	} else {
		fmt.Println("\n>>> It's a tie! No points awarded. <<<")
	}
}

func (u *consoleUI) AnnounceWinner(winner models.Participant) {
	u.ClearScr()
	name := winner.GetName()

	if !winner.IsAI() {
		fmt.Printf("🎉🎉🎉\tCongratulations, %s! You have won the game!\t🎉🎉🎉\n", name)
	} else {
		fmt.Println("☠️☠️☠️\tYou have lost the game!\t☠️☠️☠️")
	}

	u.PauseForUser()
}
