using UnityEngine;
using System.Collections;

public class GoldenPresent : MonoBehaviour {

    PlayerController pc;

    MenuManager menu;

    void Start()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        menu = GameObject.FindGameObjectWithTag("GameController").GetComponent<MenuManager>();

        if(player)
        {
            pc = player.GetComponent<PlayerController>();
        }
    }

	public void GetRandomPowerup()
    {
        int powerupIndex = Random.Range(0, 6);

        switch(powerupIndex)
        {
            case 0:
                pc.currentJetpackPower = pc.maxJetpackPower;
                menu.SetGoldenPresentText("FUEL REFIL!", Color.green);
                break;
            case 1:
                pc.UpdateLives(pc.lives + 1);
                menu.SetGoldenPresentText("EXTRA LIFE!", Color.green);
                break;
            case 2:
                pc.UpdateLives(pc.lives + 1);
                pc.currentJetpackPower = pc.maxJetpackPower;
                menu.SetGoldenPresentText("FUEL REFIL & EXTRA LIFE!", Color.green);
                break;
            case 3:
                float difficultyDuration = Random.Range(10, 30);
                GameController.Instance.oldDifficulty = GameController.Instance.difficulty;
                GameController.Instance.difficulty *= 2;
                GameController.Instance.Invoke("ResetDifficulty", difficultyDuration);
                GameController.Instance.SetGoldenSlider(difficultyDuration);
                menu.SetGoldenPresentText("DIFFICULTY INCREASE!", Color.red);
                break;
            case 4:
                float enemiesDuration = Random.Range(20, 35);
                GameController.Instance.increaseEnemies = true;
                GameController.Instance.Invoke("ResetEnemyMultplier", enemiesDuration);
                GameController.Instance.SetGoldenSlider(enemiesDuration);
                menu.SetGoldenPresentText("MORE ENEMIES!", Color.red);
                break;
            case 5:
                float pickupsDuration = Random.Range(10, 30);
                GameController.Instance.increasePickups = true;
                GameController.Instance.Invoke("ResetPickupsMultiplier", pickupsDuration);
                GameController.Instance.SetGoldenSlider(pickupsDuration);
                menu.SetGoldenPresentText("MORE PICKUPS!", Color.green);
                break;
            default:
                pc.currentJetpackPower = pc.maxJetpackPower;
                menu.SetGoldenPresentText("FUEL REFIL!", Color.green);
                break;
        }

        menu.SetUITrigger("GoldenPresent");
    }

    void RestorePlayerSpeed()
    {
        pc.moveSpeed /= 2;
    }
}
