package models

const DefaultAIName = "Opponent"

type ai struct {
	participant
}

func NewAI(name string, limit int) Participant {
	return &ai{
		participant: newParticipant(name, limit),
	}
}

func (a *ai) IsAI() bool {
	return true
}