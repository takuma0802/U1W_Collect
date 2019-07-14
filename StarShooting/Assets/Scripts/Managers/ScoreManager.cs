using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreManager : MonoBehaviour {
    public int[] CulcScore (int time, int enemy, int allEnemy, int star) {
        var timeScore = 500 * time;
        var enemyScore = 5000 * enemy * ((enemy / allEnemy) + 1);
        var starScore = 1000 * star;
        var totalScore = timeScore + enemyScore + starScore;
        return new int[] { timeScore, enemyScore, starScore, totalScore };
    }
}