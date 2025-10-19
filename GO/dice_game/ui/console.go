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

const (
	minScoreLimit     = 3
	maxScoreLimit     = 9
	minPlayerNameLen  = 4
	maxPlayerNameLen  = 15
	minPlayersAllowed = 1
	maxPlayersAllowed = 2
)

type console struct {
	reader *bufio.Reader
}

func New() UI {
	return &console{
		reader: bufio.NewReader(os.Stdin),
	}
}

func (u *console) ClearScr() {
	var cmd *exec.Cmd
	if runtime.GOOS == "windows" {
		cmd = exec.Command("cmd", "/c", "cls")
	} else {
		cmd = exec.Command("clear")
	}
	cmd.Stdout = os.Stdout
	cmd.Run()
}

func (u *console) getUserInput(prompt string) (string, error) {
	fmt.Print(prompt)
	input, err := u.reader.ReadString('\n')

	if err != nil {
		return "", err
	}

	return strings.TrimSpace(input), nil
}

func (u *console) PauseForUser() {
	fmt.Print("\nPress 'Enter' to continue...")
	u.reader.ReadString('\n')
}

func (u *console) handleInputErr(message string) {
	fmt.Println(message)
	u.PauseForUser()
}

func (u *console) printBanner(message string) {
	fmt.Printf("------- \t%s\t -------\n\n", message)
}

func (u *console) GoodbyeMsg() {
	fmt.Println("\nCome to play again...\nGoodbye!")
}

func (u *console) PlayerNamePrompt(p, pCount int) string {
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

		if name == "" || len(name) < minPlayerNameLen || len(name) > maxPlayerNameLen {
			u.handleInputErr(fmt.Sprintf("Name must be between %d and %d characters long.", minPlayerNameLen, maxPlayerNameLen))
			continue
		}

		runes := []rune(name)
		runes[0] = unicode.ToUpper(runes[0])
		return string(runes)
	}
}

func (u *console) ScoreLimitPrompt() int {
	for {
		u.ClearScr()

		input, err := u.getUserInput(fmt.Sprintf("Enter the score limit (%d - %d): ", minScoreLimit, maxScoreLimit))
		if err != nil {
			u.handleInputErr("An error occurred, please try again.")
			continue
		}

		limit, err := strconv.Atoi(input)
		if err != nil {
			u.handleInputErr("Invalid input. Please enter a whole number.")
			continue
		}

		if limit < minScoreLimit || limit > maxScoreLimit {
			u.handleInputErr(fmt.Sprintf("The limit must be between %d and %d. Please try again.", minScoreLimit, maxScoreLimit))
			continue
		}

		return limit
	}
}

func (u *console) GameMenuPrompt() int {
	for {
		u.ClearScr()
		u.printBanner("Welcome to the Dice Game!")
		u.printBanner("Please make a choice")

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

func (u *console) GameStartPrompt(scoreLimit int) {
	u.printBanner(fmt.Sprintf("Starting the game: First to collect %d points wins!", scoreLimit))
	u.PauseForUser()
}

func (u *console) RollPrompt() {
	fmt.Println(">>> Roll your dices! <<<")
	u.PauseForUser()
}

func (u *console) PlayersCountPrompt() int {
	for {
		u.ClearScr()

		input, err := u.getUserInput(fmt.Sprintf("Enter the number of players (%d - %d): ", minPlayersAllowed, maxPlayersAllowed))
		if err != nil {
			u.handleInputErr("An error occurred, please try again.")
			continue
		}

		count, err := strconv.Atoi(input)
		if err != nil {
			u.handleInputErr("Invalid input. Please enter a whole number.")
			continue
		}

		if count < minPlayersAllowed || count > maxPlayersAllowed {
			u.handleInputErr(fmt.Sprintf("The number of players must be between %d and %d. Please try again.", minPlayersAllowed, maxPlayersAllowed))
			continue
		}

		return count
	}
}

func (u *console) DisplayInfo(participants []models.Participant) {
	if len(participants) == 0 {
		return
	}
	for _, p := range participants {
		fmt.Printf(">>>\t%s - scores: %d\t||\tScore limit: %d\t<<<\n", p.GetName(), p.GetScore(), p.GetLimit())
	}
	fmt.Println()
}

func (u *console) DisplayRoundOutcome(participants []models.Participant, rollRes []models.RollResult, winnerName string) {
	u.ClearScr()
	u.DisplayInfo(participants)

	for _, r := range rollRes {
		fmt.Printf("%s rolled a %d\n", r.Participant.GetName(), r.RollValue)
	}

	if winnerName != "" {
		fmt.Printf("\n>>> %s wins the round! <<<\n", winnerName)
	} else {
		fmt.Println("\n>>> It's a tie! No points awarded. <<<")
	}
}

func (u *console) AnnounceWinner(winner models.Participant) {
	u.ClearScr()
	name := winner.GetName()

	if !winner.IsAI() {
		fmt.Printf("🎉🎉🎉\tCongratulations, %s! You have won the game!\t🎉🎉🎉\n", name)
	} else {
		fmt.Println("☠️☠️☠️\tYou have lost the game!\t☠️☠️☠️")
	}

	u.PauseForUser()
}
