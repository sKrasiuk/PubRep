package note

import (
	"encoding/json"
	"errors"
	"fmt"
	"os"
	"strings"
	"time"
)

type Note struct {
	Title     string    `json:"title"`
	Content   string    `json:"content"`
	CreatedAt time.Time `json:"created_at"`
}

func New(title, content string) (*Note, error) {
	if title == "" || content == "" {
		return nil, errors.New("invalid input")
	}

	return &Note{
		Title:     title,
		Content:   content,
		CreatedAt: time.Now().Truncate(time.Second),
	}, nil
}

func (n *Note) Print() {
	fmt.Printf("Note title: %v\n", n.Title)
	fmt.Printf("Note content: %v\n", n.Content)
	fmt.Printf("Created at: %v\n", n.CreatedAt.Format("2006-01-02 15:04:05"))
}

func (n Note) Save() error {
	fileName := strings.ReplaceAll(n.Title, " ", "_")
	fileName = strings.ToLower(fileName) + ".json"

	data, err := json.Marshal(n)

	if err != nil {
		return err
	}

	return os.WriteFile(fileName, data, 0644)
}
