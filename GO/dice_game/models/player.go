package models

type player struct {
	participant
}

func NewPlayer(name string, limit int) Participant {
	return &player{
		participant: newParticipant(name, limit),
	}
}
