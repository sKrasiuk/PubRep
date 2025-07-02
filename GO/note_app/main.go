package main

import (
	"bufio"
	"fmt"
	"note_app/interfaces"
	"note_app/note"
	"note_app/todo"
	"os"
	"os/exec"
	"runtime"
	"strconv"
	"strings"
	"time"
)

func main() {

	for {
		clearScreen()

		fmt.Println("Choose your action:")
		fmt.Println("")
		printMenu()

		switch getMenuChoice("input") {
		case 1:
			noteTitle, noteContent := getEntryData("Note title:", "Note content:")
			userNote, err := note.New(noteTitle, noteContent)

			if err != nil {
				fmt.Println(err)
				pauseForUser()
				continue
			}

			err = outputData(userNote)

			if err != nil {
				fmt.Println(err)
				pauseForUser()
				continue
			}

		case 2:
			todoTitle, todoContent := getEntryData("Todo title:", "Todo content:")
			userTodo, err := todo.New(todoTitle, todoContent)

			if err != nil {
				fmt.Println(err)
				pauseForUser()
				continue
			}
			
			err = outputData(userTodo)

			if err != nil {
				fmt.Println(err)
				pauseForUser()
				continue
			}

		case 3:
			fmt.Println("Goodbye!")
			return
		}
	}
}

func getUserInput(prompt string) string {
	fmt.Printf("%v ", prompt)

	reader := bufio.NewReader(os.Stdin)
	text, err := reader.ReadString('\n')

	if err != nil {
		return ""
	}

	// text = strings.TrimSuffix(text, "\n")
	// text = strings.TrimSuffix(text, "\r")
	text = strings.TrimSpace(text)

	return text
}

func getEntryData(titlePrompt, contentPrompt string) (string, string) {
	title := getUserInput(titlePrompt)
	content := getUserInput(contentPrompt)

	return title, content
}

func outputData(data interfaces.Outputtable) error {
	data.Print()
	err := data.Save()

	if err != nil {
		return err
	}

	fmt.Println("\nSave successful.")
	pauseForUser()
	// fmt.Println("Returning to main menu in 1 second...")
	// time.Sleep(time.Second)

	return nil
}

func printMenu() {
	fmt.Println("1. Create note")
	fmt.Println("2. Create todo")
	fmt.Println("3. Exit")
}

func getMenuChoice(prompt string) uint8 {
	var userInput string

	fmt.Printf("\n%v: ", prompt)
	fmt.Scanln(&userInput)

	value, err := strconv.ParseUint(userInput, 10, 8)

	if err != nil || value == 0 || value > 3 {
		fmt.Println("Invalid input")
		time.Sleep(300*time.Millisecond)
	}

	return uint8(value)
}

func clearScreen() {
	var cmd *exec.Cmd

	if runtime.GOOS == "windows" {
		cmd = exec.Command("cmd", "/c", "cls")
	} else {
		cmd = exec.Command("clear")
	}

	cmd.Stdout = os.Stdout
	cmd.Run()
}

func pauseForUser() {
	fmt.Print("\nPress 'Enter' to continue...")

	reader := bufio.NewReader(os.Stdin)
	reader.ReadString('\n')
}
