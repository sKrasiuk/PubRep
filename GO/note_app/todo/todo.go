package todo

import (
	"encoding/json"
	"errors"
	"fmt"
	"os"
	"strings"
)

type Todo struct {
	Title   string `json:"title"`
	Content string `json:"content"`
}

func New(title, content string) (*Todo, error) {
	if content == "" {
		return nil, errors.New("invalid input")
	}

	return &Todo{
		Title: title,
		Content: content,
	}, nil
}

func (n *Todo) Print() {
	fmt.Printf("Todo title: %v\n", n.Title)
	fmt.Printf("Todo content: %v\n", n.Content)
}

func (n Todo) Save() error {
	fileName := strings.ReplaceAll(n.Title, " ", "_")
	fileName = "todo_" + strings.ToLower(fileName) + ".json"

	data, err := json.Marshal(n)

	if err != nil {
		return err
	}

	return os.WriteFile(fileName, data, 0644)
}
