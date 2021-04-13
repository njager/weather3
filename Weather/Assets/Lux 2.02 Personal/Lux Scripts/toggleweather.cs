using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
public class toggleweather : MonoBehaviour
{


	//	WETNESS
	[Header("Dynamic Weather")]
	[Space(4)]
	public bool ScriptControlledWeather = true;

	[Space(10)]
	[Range(0.1f, 4.0f)]
	public float TimeScale = 1.0f;
	[Tooltip("Next to 'Rainfall' 'Temperature' is the most important input as it decides whether it rains or snows.'")]
	[Range(-40.0f, 40.0f)]
	public float Temperature = 20.0f;
	[Tooltip("Controls rain and snow intensity according to the given temperature.")]
	[Range(0.0f, 1.0f)]
	public float Rainfall = 0.0f;           // Snow and Rain

	[ReadOnlyRange(0.0f, 1.0f)]
	public float RainIntensity = 0.0f;      // Rain only
	[ReadOnlyRange(0.0f, 1.0f)]
	public float SnowIntensity = 0.0f;      // Snow only

	[Space(10)]
	[Range(0.0f, 1.0f)]
	public float WetnessInfluenceOnAlbedo = 0.85f;
	[Range(0.0f, 1.0f)]
	public float WetnessInfluenceOnSmoothness = 0.5f;
	[Range(0.0f, 1.0f)]
	public float AccumulationRateWetness = 0.35f;
	[Range(0.0f, 0.5f)]
	public float EvaporationRateWetness = 0.075f;

	[Space(10)]
	[Range(0.0f, 1.0f)]
	public float AccumulationRateCracks = 0.25f;
	[Range(0.0f, 1.0f)]
	public float AccumulationRatePuddles = 0.2f;

	[Range(0.0f, 0.5f)]
	public float EvaporationRateCracks = 0.20f;
	[Range(0.0f, 0.5f)]
	public float EvaporationRatePuddles = 0.1f;

	[Space(5)]
	[Range(0.0f, 0.5f)]
	public float AccumulationRateSnow = 0.10f;

	[Space(15)]
	[Range(0.0f, 1.0f)]
	public float AccumulatedWetness = 0.0f;
	[Range(0.0f, 1.0f)]
	public float AccumulatedCracks = 0.0f;
	[Range(0.0f, 1.0f)]
	public float AccumulatedPuddles = 0.0f;
	[ReadOnlyRange(0.0f, 1.0f)]
	public float AccumulatedWater = 0.0f;
	[Space(5)]
	[Range(0.0f, 1.0f)]
	public float AccumulatedSnow = 0.0f;

	[Space(5)]
	[Range(0.0f, 1.0f)]
	public float WaterToSnow = 0.01f;

	[Space(15)]
	[Range(0.01f, 1.0f)]
	public float WaterToSnowTimeScale = 1.0f;
	public AnimationCurve WaterToSnowCurve = new AnimationCurve(new Keyframe(0, 0.25f), new Keyframe(1, 1));

	[ReadOnlyRange(0.0f, 1.0f)]
	public float SnowMelt = 0.0f;

	[Header("Snow")]
	[Space(4)]
	[ColorUsageAttribute(false/*no alpha*/, true)] public Color SnowColor = Color.white;
	public Color SnowSpecularColor = new Color(0.2f, 0.2f, 0.2f, 1.0f);
	public Color SnowScatterColor;
	[Range(0.0f, 1.0f)]
	public float SnowDiffuseScatteringBias = 0.0f;
	[Range(1.0f, 10.0f)]
	public float SnowDiffuseScatteringContraction = 8.0f;
	[Space(5)]
	[Tooltip("Mask in (G) Normal in (BA)")]
	public Texture2D SnowMask;
	[Tooltip("Snow and Water Bump in (GA) Snow Smoothness in (B)")]
	public Texture2D SnowAndWaterBump;

	[Range(-100.0f, 8000.0f)]
	public float SnowStartHeight = -100.0f;
	[Range(0.0f, 1.0f)]
	public float SnowHeightBlending = 0.01f;
	private float Lux_adjustedSnowAmount;

	[Header("World mapped Snow")]
	[Space(4)]
	public float SnowTiling = 0.2f;
	public float SnowDetailTiling = 0.5f;
	public float SnowMaskTiling = 0.01f;
	public float SnowMaskDetailTiling = 0.37f;
	public float SnowNormalStregth = 1.0f;
	public float SnowNormalDetailStrength = 0.3f;


	[Space(10)]
	public ParticleSystem SnowParticleSystem;
#if UNITY_5_5_OR_NEWER
#else
	private ParticleSystem.EmissionModule SnowEmissionModule;
#endif
	public int MaxSnowParticlesEmissionRate = 3000;

	[Header("Rain")]
	[Space(4)]
	public Texture2D RainRipples;
	public float RippleTiling = 4.0f;
	public float RippleAnimSpeed = 1.0f;
	[Range(0.0f, 1.0f)]
	public float RippleRefraction = 0.5f;
	//	Offscreen rain ripples
	public int RenderTextureSize = 512;
	//public Shader RainRippleCompositeShader;
	public RenderTexture RainRipplesRenderTexture;
	private Material m_material;

	[Space(10)]
	public ParticleSystem RainParticleSystem;
#if UNITY_5_5_OR_NEWER
#else
	private ParticleSystem.EmissionModule RainEmissionModule;
#endif
	public int MaxRainParticlesEmissionRate = 3000;


	//	PIDs	
	private int RainSnowIntensityPID;
	private int WaterFloodLevelPID;
	private int RainRipplesPID; // deprecated ripple texture
	private int RippleAnimSpeedPID;
	private int RippleTilingPID;
	private int RippleRefractionPID;

	private int SnowHeightParamsPID;
	private int WaterToSnowPID;
	private int SnowMeltPID;
	private int SnowAmountPID;
	private int SnowColorPID;
	private int SnowSpecColorPID;
	private int SnowScatterColorPID;
	private int SnowScatteringBiasPID;
	private int SnowSnowScatteringContractionPID;
	private int WorldMappedSnowTilingPID;
	private int WorldMappedSnowStrengthPID;

	private int SnowMaskPID;
	private int SnowWaterBumpPID;

	[Header("GI")]
	[Space(4)]
	[Tooltip("When using dynamic GI you may attach one renderer per GI System in order to make GI being synced automatically to the given amount of snow.")]
	public Renderer[] SnowGIMasterRenderers;

	//

	private bool isDry;
	private bool isRain;
	private bool isSnow;

	void OnEnable()
	{
		SetupRippleRT();
	}

	void SetupRippleRT()
	{
		if (RainRipplesRenderTexture == null || m_material == null)
		{
			RainRipplesRenderTexture = new RenderTexture(RenderTextureSize, RenderTextureSize, 0, RenderTextureFormat.ARGB32, RenderTextureReadWrite.Linear);
			RainRipplesRenderTexture.useMipMap = true;
			RainRipplesRenderTexture.wrapMode = TextureWrapMode.Repeat;
			m_material = new Material(Shader.Find("Hidden/Lux RainRipplesComposite")); //new Material(RainRippleCompositeShader);

			GetPIDs();
		}
	}

	void GetPIDs()
	{
		// Rain
		RainSnowIntensityPID = Shader.PropertyToID("_Lux_RainfallRainSnowIntensity");
		WaterFloodLevelPID = Shader.PropertyToID("_Lux_WaterFloodlevel");
		RainRipplesPID = Shader.PropertyToID("_Lux_RainRipples");
		RippleAnimSpeedPID = Shader.PropertyToID("_Lux_RippleAnimSpeed");
		RippleTilingPID = Shader.PropertyToID("_Lux_RippleTiling");
		RippleRefractionPID = Shader.PropertyToID("_Lux_RippleRefraction");

		// Snow
		SnowHeightParamsPID = Shader.PropertyToID("_Lux_SnowHeightParams");
		WaterToSnowPID = Shader.PropertyToID("_Lux_WaterToSnow");
		SnowMeltPID = Shader.PropertyToID("_Lux_SnowMelt");
		SnowAmountPID = Shader.PropertyToID("_Lux_SnowAmount");
		SnowColorPID = Shader.PropertyToID("_Lux_SnowColor");
		SnowSpecColorPID = Shader.PropertyToID("_Lux_SnowSpecColor");

		SnowScatterColorPID = Shader.PropertyToID("_Lux_SnowScatterColor");
		SnowScatteringBiasPID = Shader.PropertyToID("_Lux_SnowScatteringBias");
		SnowSnowScatteringContractionPID = Shader.PropertyToID("_Lux_SnowScatteringContraction");
		WorldMappedSnowTilingPID = Shader.PropertyToID("_Lux_WorldMappedSnowTiling");
		WorldMappedSnowStrengthPID = Shader.PropertyToID("_Lux_WorldMappedSnowStrength");

		SnowMaskPID = Shader.PropertyToID("_Lux_SnowMask");
		SnowWaterBumpPID = Shader.PropertyToID("_Lux_SnowWaterBump");
	}

	//void Start()
	//{
	//	isDry = true;
	//	isSnow = false;
	//	isRain = false;

	//}



	//void Raining()
	//{
	//	if (isSnow == true)
		//{
			//snow to rain 
	//		Mathf.Lerp(-40.0f, Temperature / 0.0f);
	//		Mathf.Lerp(1.0f, AccumulatedSnow / 0.0f);
	//		Mathf.Lerp(0.0f, AccumulatedCracks, AccumulatedWetness, AccumulatedPuddles / 1.0f);
	//		Mathf.Lerp(1.0f, WaterToSnow / 0.0f);
	//		isDry = false;
	//		isRain = true;
	//		isSnow = false;
	//	}

		//if (isDry == true)
		//{
			//dry to rain
		//	Mathf.Lerp(40.0f, Temperature / 0.0f);
		//	Mathf.Lerp(0.0f, AccumulatedCracks, AccumulatedWetness, AccumulatedPuddles / 1.0f);
		//	isDry = false;
		//	isRain = true;
		//	isSnow = false;
		//}
	//}

	//void Snowing()
	//{
		//if (isRain == true)
		//{
			//rain to snow
			//Mathf.Lerp(0.0f, AccumulatedSnow / 1.0f);
			//Mathf.Lerp(1.0f, AccumulatedCracks, AccumulatedWetness, AccumulatedPuddles / 0.0f);
			//Mathf.Lerp(0.0f, WaterToSnow / 1.0f);
			//isDry = false;
			//isRain = false;
			//isSnow = true;
		//}

		//if (isDry == true)
		//{
			//dry to snow
			//Mathf.Lerp(40.0f, Temperature / -40.0f);
			//Mathf.Lerp(0.0f, AccumulatedSnow / 1.0f);
			//Mathf.Lerp(0.0f, WaterToSnow / 1.0f);
			//isDry = false;
			//isRain = false;
			//isSnow = true;
		//}
	//}

	//void Drying()
	//{
		//if (isSnow == true)
		//{
			//snow to dry 
			//Mathf.Lerp(-40.0f, Temperature / 40.0f);
			//Mathf.Lerp(1.0f, AccumulatedSnow / 0.0f);
			//Mathf.Lerp(1.0f, WaterToSnow / 0.0f);
			//isDry = true;
			//isRain = false;
			//isSnow = false;
		//}

		//if (isRain == true)
		//{
			//rain to dry 
			//Mathf.Lerp(0.0f, Temperature / 40.0f);
			//Mathf.Lerp(1.0f, AccumulatedCracks, AccumulatedWetness, AccumulatedPuddles / 0.0f);
			//isDry = true;
			//isRain = false;
			//isSnow = false;
		//}
	//}


	//private void FixedUpdate()
	//{
		//if (Input.GetKeyDown(KeyCode.R))
		//{
			//Raining();
		//}

		//if (Input.GetKeyDown(KeyCode.T))
		//{
			//Drying();
		//}

		//if (Input.GetKeyDown(KeyCode.Y))
		//{
			//Snowing();
		//}

	//}

}
	
