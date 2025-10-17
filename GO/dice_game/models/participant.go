package models

type participant struct {
	name  string
	score int
	dices [2]*dice
	limit int
}

func newParticipant(name string, limit int) participant {
	return participant{
		name:  name,
		score: 0,
		dices: [2]*dice{newDice(), newDice()},
		limit: limit,
	}
}

func (p *participant) RollDices() int {
	return p.dices[0].roll() + p.dices[1].roll()
}

func (p *participant) GetName() string {
	return p.name
}

func (p *participant) GetScore() int {
	return p.score
}

func (p *participant) AddToScore(points int) {
	p.score += points
}

func (p *participant) GetLimit() int {
	return p.limit
}

func (p *participant) IsAI() bool {
	return false
}
