package models

type Participant interface {
	RollDices() int
	GetName() string
	GetScore() int
	AddToScore(p int)
	GetLimit() int
	IsAI() bool
}