using TMPro;
using UnityEngine;

namespace UnityArchitecture.SpaghettiPattern
{
    public class UIStatisticsMenu : MonoBehaviour
    {
        [Header("UI")]
        public Color statisticColor;
        public Color labelColor;

        public TextMeshProUGUI killText;
        public TextMeshProUGUI bossKillText;
        public TextMeshProUGUI goldText;
        public TextMeshProUGUI chestText;
        public TextMeshProUGUI deathText;
        public TextMeshProUGUI damageDealtText;
        public TextMeshProUGUI damageTakenText;
        public TextMeshProUGUI damageHealedText;
        public TextMeshProUGUI gamesWonText;
        public TextMeshProUGUI gamesPlayedText;
        public TextMeshProUGUI fastestWinText;
        public TextMeshProUGUI highestPistolDamageText;
        public TextMeshProUGUI highestPistolRangeText;
        public TextMeshProUGUI highestPistolFireRateText;
        public TextMeshProUGUI highestPistolKnockBackText;
        public TextMeshProUGUI highestPistolPierceText;
        public TextMeshProUGUI highestPlayerSpeedText;
        public TextMeshProUGUI highestPlayerHealthText;
        public TextMeshProUGUI highestSwordDamageText;
        public TextMeshProUGUI highestSwordRangeText;
        public TextMeshProUGUI highestSwordAttackSpeedText;
        public TextMeshProUGUI highestSwordKnockBackText;
        public TextMeshProUGUI highestSwordArcText;
        public TextMeshProUGUI highestHealthPackSpawnRateText;
        public TextMeshProUGUI highestLuckText;
        public TextMeshProUGUI highestBlockText;

        public void UpdateUI()
        {
            var label = ColorUtility.ToHtmlStringRGB(labelColor);
            var stat = ColorUtility.ToHtmlStringRGB(statisticColor);
            killText.text = $"<color=#{label}>Enemies Killed:</color> <color=#{stat}>{AccountManager.instance.statistics.totalKills}</color>";
            bossKillText.text = $"<color=#{label}>Bosses Killed:</color> <color=#{stat}>{AccountManager.instance.statistics.totalBossKills}</color>";
            goldText.text = $"<color=#{label}>Gold Earnt:</color> <color=#{stat}>{AccountManager.instance.statistics.totalGoldEarned}</color>";
            chestText.text = $"<color=#{label}>Chests Opened:</color> <color=#{stat}>{AccountManager.instance.statistics.totalChestsOpened}</color>";
            deathText.text = $"<color=#{label}>Deaths:</color> <color=#{stat}>{AccountManager.instance.statistics.totalDeaths}</color>";
            damageDealtText.text = $"<color=#{label}>Damage Dealt:</color> <color=#{stat}>{AccountManager.instance.statistics.totalDamageDealt}</color>";
            damageTakenText.text = $"<color=#{label}>Damage Taken:</color> <color=#{stat}>{AccountManager.instance.statistics.totalDamageTaken}</color>";
            damageHealedText.text = $"<color=#{label}>Damage Healed:</color> <color=#{stat}>{AccountManager.instance.statistics.totalDamageHealed}</color>";
            gamesWonText.text = $"<color=#{label}>Games Won:</color> <color=#{stat}>{AccountManager.instance.statistics.gamesWon}</color>";
            gamesPlayedText.text = $"<color=#{label}>Games Played:</color> <color=#{stat}>{AccountManager.instance.statistics.gamesPlayed}</color>";
            fastestWinText.text = $"<color=#{label}>Fastest Win:</color> <color=#{stat}>{AccountManager.instance.statistics.fastestWin}</color>";

            highestPistolDamageText.text =
                $"<color=#{label}>Highest Pistol Damage:</color> <color=#{stat}>{AccountManager.instance.statistics.highestPistolDamage}</color>";
            highestPistolRangeText.text =
                $"<color=#{label}>Highest Pistol Range:</color> <color=#{stat}>{AccountManager.instance.statistics.highestPistolRange}</color>";
            highestPistolFireRateText.text =
                $"<color=#{label}>Highest Pistol Fire Rate:</color> <color=#{stat}>{AccountManager.instance.statistics.highestPistolFireRate}</color>";
            highestPistolKnockBackText.text =
                $"<color=#{label}>Highest Pistol Knock Back:</color> <color=#{stat}>{AccountManager.instance.statistics.highestPistolKnockBack}</color>";
            highestPistolPierceText.text =
                $"<color=#{label}>Highest Pistol Pierce:</color> <color=#{stat}>{AccountManager.instance.statistics.highestPistolPierce}</color>";
            highestPlayerSpeedText.text =
                $"<color=#{label}>Highest Player Speed:</color> <color=#{stat}>{AccountManager.instance.statistics.highestPlayerSpeed}</color>";
            highestPlayerHealthText.text =
                $"<color=#{label}>Highest Player Health:</color> <color=#{stat}>{AccountManager.instance.statistics.highestPlayerHealth}</color>";
            highestSwordDamageText.text =
                $"<color=#{label}>Highest Sword Damage:</color> <color=#{stat}>{AccountManager.instance.statistics.highestSwordDamage}</color>";
            highestSwordRangeText.text =
                $"<color=#{label}>Highest Sword Range:</color> <color=#{stat}>{AccountManager.instance.statistics.highestSwordRange}</color>";
            highestSwordAttackSpeedText.text =
                $"<color=#{label}>Highest Sword Attack Speed:</color> <color=#{stat}>{AccountManager.instance.statistics.highestSwordAttackSpeed}</color>";
            highestSwordKnockBackText.text =
                $"<color=#{label}>Highest Sword Knock Back:</color> <color=#{stat}>{AccountManager.instance.statistics.highestSwordKnockBack}</color>";
            highestSwordArcText.text =
                $"<color=#{label}>Highest Sword Arc:</color> <color=#{stat}>{AccountManager.instance.statistics.highestSwordArc}</color>";
            highestHealthPackSpawnRateText.text =
                $"<color=#{label}>Highest Health Pack Spawn Rate:</color> <color=#{stat}>{AccountManager.instance.statistics.highestHealthPackSpawnRate}</color>";
            highestLuckText.text =
                $"<color=#{label}>Highest Luck:</color> <color=#{stat}>{AccountManager.instance.statistics.highestLuck}</color>";
            highestBlockText.text =
                $"<color=#{label}>Highest Block:</color> <color=#{stat}>{AccountManager.instance.statistics.highestBlock}</color>";
        }

        private void OnEnable()
        {
            UpdateUI();
        }
    }
}