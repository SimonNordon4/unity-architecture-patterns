using UnityEngine;

namespace GameObjectComponent.Game
{
    public class RoundEndGoldHandler : MonoBehaviour
    {
        [SerializeField]private Gold characterGold;
        [SerializeField]private RoundSpawner roundSpawner;
        [SerializeField]private GameState gameState;

        [SerializeField] private int baseGold = 100;
        
        public int lastRoundGoldAdded { get; private set; }

        private void OnEnable()
        {
            gameState.OnGameWon += OnGameWon;
            gameState.OnGameLost += OnGameLost;
        }


        private void OnDisable()
        {
            gameState.OnGameWon -= OnGameWon;
            gameState.OnGameLost -= OnGameLost;
        }
        
        private void OnGameWon()
        {
            lastRoundGoldAdded = 10000;
            characterGold.ChangeGold(lastRoundGoldAdded);            
        }

        private void OnGameLost()
        { 
            Debug.Log("Game Lost, calculating Gold.");
           lastRoundGoldAdded = 0;
            
            var actorsThisWave = roundSpawner.totalActorsInRound;
            
            Debug.Log("Actors this wave: " + actorsThisWave);
            var actorGold = (float)baseGold / (float)actorsThisWave;
            
            Debug.Log("Actor Gold: " + actorGold);
            
            var roundsCompleted = roundSpawner.wavesCompleted;
            
            Debug.Log("Rounds Completed: " + roundsCompleted);

            // Add gold for every round won.
            for (var i = 0; i < roundsCompleted; i++)
            {
                lastRoundGoldAdded += baseGold * i;
            }
            
            Debug.Log("Gold added for rounds completed: " + lastRoundGoldAdded);
            Debug.Log("Actors killed this round: " + roundSpawner.actorKilledThisRound);
            // Add gold for each enemy killed this round.
            lastRoundGoldAdded += (int)actorGold * roundSpawner.actorKilledThisRound;
            
            characterGold.ChangeGold(lastRoundGoldAdded);
        }
    }
}