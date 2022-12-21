using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Score 
{
	public User user;
	public int score;
	public Enums.TrendType trendType;
    public int apiVersion;
	public int position;
	public float prize;
	public int currencyType;
}
