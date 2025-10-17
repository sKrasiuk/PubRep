package ui

import "dice_game/models"

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