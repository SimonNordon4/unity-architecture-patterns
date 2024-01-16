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
           
           lastRoundGoldAdded = 0;
            
            var actorsThisWave = roundSpawner.totalActorsInRound;

            var actorGold = (float)baseGold / (float)actorsThisWave;
            
            var roundsCompleted = roundSpawner.wavesCompleted;

            // Add gold for every round won.
            for (var i = 0; i < roundsCompleted; i++)
            {
                lastRoundGoldAdded += baseGold * i;
            }
            // Add gold for each enemy killed this round.
            lastRoundGoldAdded += (int)actorGold * roundSpawner.actorKilledThisRound;
            
            characterGold.ChangeGold(lastRoundGoldAdded);
        }
    }
}