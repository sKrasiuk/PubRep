package models

import (
	"math/rand/v2"
)

type dice struct{}

func newDice() dice {
	return dice{}
}

func (d dice) roll() int {
	return rand.IntN(6) + 1
}
