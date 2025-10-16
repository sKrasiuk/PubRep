package logic

import (
	"dice_game/models"
	"dice_game/ui"
	"sync"
)

type game struct {
	participants []models.Participant
	ui           ui.UI
}

func newGame(participants []models.Participant, ui ui.UI) *game {
	return &game{
		participants: participants,
		ui:           ui,
	}
}

func Start() {
	ui := ui.New()

	for {
		choice := ui.GameMenuPrompt()

		switch choice {
		case 0:
			ui.GoodbyeMsg()
			return
		case 1:
			game := setupNewGame(ui)
			game.run()
		}
	}
}

func setupNewGame(ui ui.UI) *game {
	playersCount := ui.PlayersCountPrompt()
	scoreLimit := ui.ScoreLimitPrompt()
	participants := make([]models.Participant, 0, 2)

	for i := 1; i <= playersCount; i++ {
		pName := ui.PlayerNamePrompt(i, playersCount)
		p := models.NewPlayer(pName, scoreLimit)
		participants = append(participants, p)
	}

	if playersCount == 1 {
		ai := models.NewAI(models.DefaultAIName, scoreLimit)
		participants = append(participants, ai)
	}

	return newGame(participants, ui)
}

func (g *game) run() {
	g.ui.ClearScr()
	scoreLimit := g.participants[0].GetLimit()
	g.ui.GameStartPrompt(scoreLimit)

	for {
		g.ui.ClearScr()
		g.ui.DisplayInfo(g.participants)
		g.ui.RollPrompt()

		var wg sync.WaitGroup
		resultsChan := make(chan models.RollResult, len(g.participants))

		for _, p := range g.participants {
			wg.Add(1)
			go func(p models.Participant) {
				defer wg.Done()
				resultsChan <- models.RollResult{Participant: p, RollValue: p.RollDices()}
			}(p)
		}

		wg.Wait()
		close(resultsChan)

		allRolls := make([]models.RollResult, 0, len(g.participants))

		for result := range resultsChan {
			allRolls = append(allRolls, result)
		}

		roundWinner := findRoundWinner(allRolls)
		
		winnerName := ""
		if roundWinner != nil {
			roundWinner.AddToScore(1)
			winnerName = roundWinner.GetName()
		}

		g.ui.DisplayRoundOutcome(g.participants, allRolls, winnerName)
		gameWinner := findGameWinner(g.participants)

		if gameWinner != nil {
			g.ui.AnnounceWinner(gameWinner)
			return
		}

		g.ui.PauseForUser()
	}
}

func findRoundWinner(allRolls []models.RollResult) models.Participant {
	if len(allRolls) == 0 {
		return nil
	}

	highestRoll := -1
	var winner models.Participant

	for _, roll := range allRolls {
		if roll.RollValue > highestRoll {
			highestRoll = roll.RollValue
			winner = roll.Participant
		} else if roll.RollValue == highestRoll {
			winner = nil
		}
	}

	return winner
}

func findGameWinner(participants []models.Participant) models.Participant {
	var gameWinner models.Participant
	highestScore := -1

	for _, p := range participants {
		if p.GetScore() >= p.GetLimit() {
			if p.GetScore() > highestScore {
				highestScore = p.GetScore()
				gameWinner = p
			}
		}
	}

	return gameWinner
}
