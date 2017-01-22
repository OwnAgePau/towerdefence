using UnityEngine;
using System.Collections;

public class TowerUpgrade : MonoBehaviour {
    
    public Upgrade[] upgrades;
    public bool hasEndlessUpgrades;
    public int maxProjectiles = 0;
    private Tower tower;
    public HoverInfo hoverInfo;
    public int maxLevel = 10;

    private float dmgToScaleFrom = 0;
    private float projToScaleFrom = 0;
    private float costToScaleFrom = 0;

    private float damageEndMod = 0.40f; // Damage should scale with adding a percentage of damage for every level
    private float projectilesEndMod = 1.0f; // Projectiles should scale with +1 only
    private float costEndMod = 0.40f;
    
    private int currentUpgradeType = 0; // 0 == Damage, 1 == Projnr
    private int upgradesTillProjUpgr = 3;

    void Start(){
        if (GUIcontroller.instance != null) {
            this.hoverInfo = GUIcontroller.instance.hoverUpgradeObject;
        }
        
        this.tower = this.gameObject.transform.parent.gameObject.GetComponent<Tower>();
        // Set damage to scale from and proj to scale from
        // Not sure if this part is still needed
        foreach (Upgrade upgrade in this.upgrades){
            foreach (UpgradePart part in upgrade.upgrades){
                if (part.type.Equals(UpgradeType.DAMAGE))
                {
                    this.dmgToScaleFrom = part.amount;
                }
                else if (part.type.Equals(UpgradeType.PROJNR))
                {
                    this.projToScaleFrom = part.amount;
                }
            }
            this.costToScaleFrom = upgrade.upgradeCost;
        }  
    }

    public Upgrade GetNextUpgrade(int towerLevel){
        int upgradeTo = towerLevel;
        if(upgradeTo >= maxLevel){
            return null;
        }
        if (upgradeTo > upgrades.Length - 1){
            // Create an endless upgrade
            Upgrade newUpgrade = this.CreateEndlessUpgrade(towerLevel);            
            // Add a new upgrade to the list, the next time it runs through this statement again as its level is equal to the list size
            return newUpgrade;
        }
        else{
            // Return an upgrade from the upgrade list
            Upgrade upgrade = this.upgrades[upgradeTo];
            hoverInfo.aspireCost = (int)upgrade.upgradeCost; // THis should not be in this class, this is GUI stuff
            return upgrade;
        }
    }

    public Upgrade UpgradeTower(int towerLevel){
        int upgradeTo = towerLevel;
        if (upgradeTo > upgrades.Length - 1){
            // Create a new upgrade if that is allowed for the tower
            if (this.hasEndlessUpgrades){
                if (this.upgradesTillProjUpgr > 0){
                    this.upgradesTillProjUpgr -= 1;
                    this.currentUpgradeType = 0;
                }
                else{
                    this.upgradesTillProjUpgr = 3;
                    this.currentUpgradeType = 1;
                }
                Upgrade upgrade = this.GetNextUpgrade(towerLevel);
                if (upgrade != null){
                    if (Player.instance.aspirePoints >= upgrade.upgradeCost){
                        // Upgrade tower and remove cost
                        Player.instance.RemoveAspirePoints(upgrade.upgradeCost);
                        this.UpgradeTowerStats(upgrade, towerLevel);
                        this.costToScaleFrom = upgrade.upgradeCost;
                    }
                }
                return upgrade;
            }
            /// Temp, when an endless upgrade is not enabled the button should not be interactable
            // this.upgrades[upgrades.Length - 1];
            return null;
        }
        else {
            Upgrade upgrade = this.upgrades[towerLevel];
            // Check costs
            if (Player.instance.aspirePoints >= upgrade.upgradeCost){
                // Upgrade tower and remove cost
                Player.instance.RemoveAspirePoints(upgrade.upgradeCost);
                this.UpgradeTowerStats(upgrade, towerLevel);
                this.costToScaleFrom = upgrade.upgradeCost;
            }
            return upgrade;
        }
    }

    public void UpgradeTowerStats(Upgrade upgrade, int towerLevel){
        foreach (UpgradePart part in upgrade.upgrades){
            switch (part.type){
                case UpgradeType.DAMAGE:
                    // Update the damage
                    tower.damage = part.amount;
                    this.dmgToScaleFrom = part.amount;
                    break;
                case UpgradeType.PROJNR:
                    // Update the projectile amounts
                    tower.projectiles = part.amount;
                    this.projToScaleFrom = part.amount;
                    break;
                case UpgradeType.SLOW:
                    // Update the slow #
                    float slow = part.amount;
                    tower.slowAmount = slow / 100;
                    break;
                case UpgradeType.SPEED:
                    // Update the speed at which the tower shoots bullets
                    tower.cooldown = part.amount;
                    break;
                case UpgradeType.RANGE:
                    tower.range = part.amount;
                    break;
            }
        }
    }

    public Upgrade CreateEndlessUpgrade(int towerLevel) {
        float currentDamage = (this.damageEndMod * this.dmgToScaleFrom) + this.dmgToScaleFrom;
        float currentProjectile = this.projToScaleFrom + 1;
        float currentSpeed = 1;
        float currentRange = 1;
        float currentCost = (this.costEndMod * this.costToScaleFrom) + this.costToScaleFrom;
        hoverInfo.aspireCost = (int)currentCost;
        //Debug.Log("Cost Scale From : " + this.costToScaleFrom + ", " + (this.costToScaleFrom * this.costEndMod) + ", towerLevel : " + towerLevel + ", total : " + currentCost);
        // Determine what upgrade is being done (damage / proj)
        UpgradePart[] parts = new UpgradePart[1];
        if (this.currentUpgradeType.Equals(0))
        {
            // Damage
            parts[0] = new UpgradePart(UpgradeType.DAMAGE, (int)currentDamage);
        }
        else if (this.currentUpgradeType.Equals(1))
        {
            // Projectile
            parts[0] = new UpgradePart(UpgradeType.PROJNR, (int)currentProjectile);
        }
        else if (this.currentUpgradeType.Equals(3))
        {
            parts[0] = new UpgradePart(UpgradeType.SPEED, (int)currentSpeed);
        }
        else if (this.currentUpgradeType.Equals(4)) {
            parts[0] = new UpgradePart(UpgradeType.RANGE, (int)currentRange);
        }

        return new Upgrade(parts, towerLevel + 1, (int)currentCost);
    }
}

[System.Serializable]
public class Upgrade
{
    public UpgradePart[] upgrades;
    public int level;
    public int upgradeCost;

    public Upgrade(UpgradePart[] upgrades, int level, int cost){
        this.upgrades = upgrades;
        this.level = level;
        this.upgradeCost = cost;
    }
}

[System.Serializable]
public class UpgradePart{
    public UpgradeType type;
    public int amount;
    
    public UpgradePart(UpgradeType type, int amount) {
        this.type = type;
        this.amount = amount;
    }
}

public enum UpgradeType{
    DAMAGE,
    SPEED,
    PROJNR,
    SLOW,
    RANGE,
}
