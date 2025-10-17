package logic

import (
	"dice_game/models"
	"testing"
)

type mockParticipant struct {
	name  string
	score int
	limit int
}

func (m *mockParticipant) RollDices() int   { return 0 }
func (m *mockParticipant) GetName() string  { return m.name }
func (m *mockParticipant) GetScore() int    { return m.score }
func (m *mockParticipant) AddToScore(p int) { m.score += p }
func (m *mockParticipant) GetLimit() int    { return m.limit }
func (m *mockParticipant) IsAI() bool       { return false }

func TestFindRoundWinner(t *testing.T) {
	p1 := &mockParticipant{name: "Player 1"}
	p2 := &mockParticipant{name: "Player 2"}

	testCases := []struct {
		name           string
		allRolls       []models.RollResult
		expectedWinner models.Participant
	}{
		{
			name: "Player1 wins with a highter roll",
			allRolls: []models.RollResult{
				{Participant: p1, RollValue: 7},
				{Participant: p2, RollValue: 5},
			},
			expectedWinner: p1,
		},
		{
			name: "Player2 wins with a highter roll",
			allRolls: []models.RollResult{
				{Participant: p1, RollValue: 5},
				{Participant: p2, RollValue: 7},
			},
			expectedWinner: p2,
		},
		{
			name: "A tie results. No winner",
			allRolls: []models.RollResult{
				{Participant: p1, RollValue: 5},
				{Participant: p2, RollValue: 5},
			},
			expectedWinner: nil,
		},
		{
			name:           "Edge case. An empty list of roll resuls. No winner",
			allRolls:       []models.RollResult{},
			expectedWinner: nil,
		},
	}

	for _, tc := range testCases {
		t.Run(tc.name, func(t *testing.T) {
			actualWinner := findRoundWinner(tc.allRolls)

			if actualWinner != tc.expectedWinner {
				t.Errorf("findRoundWinner(%v) = %v, want %v", tc.allRolls, actualWinner, tc.expectedWinner)
			}
		})
	}
}

func TestFindGameWinner(t *testing.T) {
	const scoreLimit = 3

	testCases := []struct {
		name           string
		setup          func() []models.Participant
		expectedWinner models.Participant
	}{
		{
			name: "Player1 wins by reaching score limit first",
			setup: func() []models.Participant {
				p1 := &mockParticipant{name: "Player 1", score: 3, limit: scoreLimit}
				p2 := &mockParticipant{name: "Player 2", score: 0, limit: scoreLimit}
				return []models.Participant{p1, p2}
			},
			expectedWinner: &mockParticipant{name: "Player 1"},
		},
		{
			name: "Player2 wins by reaching score limit first",
			setup: func() []models.Participant {
				p1 := &mockParticipant{name: "Player 1", score: 0, limit: scoreLimit}
				p2 := &mockParticipant{name: "Player 2", score: 3, limit: scoreLimit}
				return []models.Participant{p1, p2}
			},
			expectedWinner: &mockParticipant{name: "Player 2"},
		},
		{
			name: "No winner. Scores limit not reached",
			setup: func() []models.Participant {
				p1 := &mockParticipant{name: "Player 1", score: 0, limit: scoreLimit}
				p2 := &mockParticipant{name: "Player 2", score: 0, limit: scoreLimit}
				return []models.Participant{p1, p2}
			},
			expectedWinner: nil,
		},
		{
			name:           "Edge case. No winner. Empty list of participants",
			setup:          func() []models.Participant { return []models.Participant{} },
			expectedWinner: nil,
		},
	}

	for _, tc := range testCases {
		t.Run(tc.name, func(t *testing.T) {
			participants := tc.setup()
			actualWinner := findGameWinner(participants)

			if actualWinner == nil && tc.expectedWinner == nil {
				return
			}

			if actualWinner == nil || tc.expectedWinner == nil {
				t.Errorf("findGameWinner() got winner: %v, want %v", actualWinner, tc.expectedWinner)
			} else if actualWinner.GetName() != tc.expectedWinner.GetName() {
				t.Errorf("findGameWinner() got winner name: %v, want %v", actualWinner.GetName(), tc.expectedWinner.GetName())
			}
		})
	}
}
