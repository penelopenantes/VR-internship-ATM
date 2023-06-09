﻿//2016 Spyblood Games

using UnityEngine;

namespace AI_Package
{
	[System.Serializable]
	public class DayColors
	{
		public Color skyColor;
		public Color equatorColor;
		public Color horizonColor;
	}

	public class DayAndNightControl : MonoBehaviour
	{
		public bool StartDay; //start game as day time
		public GameObject StarDome;
		public GameObject moonState;
		public GameObject moon;
		public DayColors dawnColors;
		public DayColors dayColors;
		public DayColors nightColors;
		public int currentDay = 0;
		public Light directionalLight; //the directional light in the scene we're going to work with
		public float SecondsInAFullDay = 120f; //in realtime, this is about two minutes by default. (every 1 minute/60 seconds is day in game)
		[Range(0, 1)]
		public float currentTime = 0; //at default when you press play, it will be nightTime. (0 = night, 1 = day)
		[HideInInspector]
		public float timeMultiplier = 1f; //how fast the day goes by regardless of the secondsInAFullDay var. lower values will make the days go by longer, while higher values make it go faster. This may be useful if you're siumulating seasons where daylight and night times are altered.
		public bool showUI;
		float lightIntensity; //static variable to see what the current light's insensity is in the inspector
		Material starMat;

		Camera targetCam;

		public delegate void OnMorningListener();
		public event OnMorningListener OnMorningHandler;

		public delegate void OnEveningListener();
		public event OnEveningListener OnEveningHandler;


		private bool dayCall = true;
		private bool nightCall = true;
		// Use this for initialization
		void Start()
		{
			foreach (Camera c in GameObject.FindObjectsOfType<Camera>())
			{
				if (c.isActiveAndEnabled)
				{
					targetCam = c;
				}
			}
			lightIntensity = directionalLight.intensity; //what's the current intensity of the light
			starMat = StarDome.GetComponentInChildren<MeshRenderer>().material;
			if (StartDay)
			{
				currentTime = 0.3f; //start at morning
				starMat.color = new Color(1f, 1f, 1f, 0f);
			}
		}

		// Update is called once per frame
		void Update()
		{
			UpdateLight();

			currentTime += (Time.deltaTime / SecondsInAFullDay) * timeMultiplier;
			if (currentTime >= 1)
			{
				currentTime = 0;
				dayCall = true;
				nightCall = true;
			}
			if (currentTime < 0.5f && currentTime > 0.3f && dayCall)
			{
				OnMorningHandler?.Invoke();
				dayCall = false;
			}
			if (currentTime > 0.7f && nightCall)
			{
				OnEveningHandler?.Invoke();
				nightCall = false;
			}
		}

		void UpdateLight()
		{
			StarDome.transform.Rotate(new Vector3(0, 2f * Time.deltaTime, 0));
			moon.transform.LookAt(targetCam.transform);
			directionalLight.transform.localRotation = Quaternion.Euler((currentTime * 360f) - 90, 170, 0);
			moonState.transform.localRotation = Quaternion.Euler((currentTime * 360f) - 100, 170, 0);
			//^^ we rotate the sun 360 degrees around the x axis, or one full rotation times the current time variable. we subtract 90 from this to make it go up
			//in increments of 0.25.

			//the 170 is where the sun will sit on the horizon line. if it were at 180, or completely flat, it would be hard to see. Tweak this value to what you find comfortable.

			float intensityMultiplier = 1;

			if (currentTime <= 0.23f || currentTime >= 0.75f)
			{
				intensityMultiplier = 0; //when the sun is below the horizon, or setting, the intensity needs to be 0 or else it'll look weird
				starMat.color = new Color(1, 1, 1, Mathf.Lerp(1, 0, Time.deltaTime));
				SetColors(nightColors);
			}
			else if (currentTime <= 0.25f)
			{
				intensityMultiplier = Mathf.Clamp01((currentTime - 0.23f) * (1 / 0.02f));
				starMat.color = new Color(1, 1, 1, Mathf.Lerp(0, 1, Time.deltaTime));
				SetColors(dayColors);
			}
			else if (currentTime <= 0.73f)
			{
				intensityMultiplier = Mathf.Clamp01(1 - ((currentTime - 0.73f) * (1 / 0.02f)));
				SetColors(dayColors);
			}

			directionalLight.intensity = lightIntensity * intensityMultiplier;
		}

		void SetColors(DayColors dayColors)
        {
			RenderSettings.ambientSkyColor = dayColors.skyColor;
			RenderSettings.ambientEquatorColor = dayColors.equatorColor;
			RenderSettings.ambientGroundColor = dayColors.horizonColor;
		}

		public string TimeOfDay()
		{
			string dayState = "";
			if (currentTime > 0f && currentTime < 0.3f)
			{
				dayState = "Midnight";
			}
			else if (currentTime < 0.5f && currentTime > 0.3f)
			{
				dayState = "Morning";
			}
			else if (currentTime > 0.5f && currentTime < 0.6f)
			{
				dayState = "Mid Noon";
			}
			else if (currentTime > 0.7f && currentTime < 0.8f)
			{
				dayState = "Evening";
			}
			else if (currentTime > 0.8f && currentTime < 1f)
			{
				dayState = "Night";
			}

			return dayState;
		}
	}
}
