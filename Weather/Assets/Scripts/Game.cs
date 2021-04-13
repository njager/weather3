using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Game : MonoBehaviour
{
    //private variables
    private Rigidbody rB;
    bool isRaining;
    bool isDry;
    bool isSnowing;
    bool haveSeeds;
    bool seedsPlanted;
    bool plantGrown;
    bool havePlant;
    bool generatorOn;
    bool leverPulled;
    bool windmillOn;
    bool hasFuze;
    bool fuzeCharged;
    bool plugIn;
    bool bridgeUp;

    //public variables
    [SerializeField] GameObject iceRiver;
    [SerializeField] GameObject wetRiver;
    [SerializeField] GameObject bridge;
    [SerializeField] GameObject plank;
    [SerializeField] GameObject plant;
    [SerializeField] List<GameObject> waterObjects;
    [SerializeField] Material ice;
    [SerializeField] Material runnyWater;
    public static CursorLockMode lockState;

    // Start is called before the first frame update
    void Start()
    {
        rB = GetComponent<Rigidbody>();
        isDry = true;
        Cursor.lockState = CursorLockMode.Locked;
    }

    //called last every frame
    private void FixedUpdate()
    {
        //detect mouse input
        if (Input.GetMouseButtonDown(0))
        {
            Debug.Log("mouse clicked!");

            //send out a raycast to detect collisions
            Ray interactionRay = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit interactionInfo;
            if (Physics.Raycast(interactionRay, out interactionInfo, Mathf.Infinity))
            {
                //check the object interacted with
                GameObject interactedObject = interactionInfo.collider.gameObject;

                //rune interactions
                if (interactedObject.tag == "RuneRain")
                {
                    //if changing weather, check previous weather and adjust water level 
                    if (!isRaining == true)
                    {
                        if (isDry == true)
                        {
                            foreach (GameObject obj in waterObjects)
                            {
                                obj.gameObject.transform.localScale += new Vector3(0, 2, 0);
                                MeshRenderer meshRenderer;
                                meshRenderer = obj.GetComponent<MeshRenderer>();
                                meshRenderer.material = runnyWater;
                            }
                            Debug.Log("Water goes higher");
                        }
                        else if (isSnowing == true)
                        {
                            foreach (GameObject obj in waterObjects)
                            {
                                obj.gameObject.transform.localScale += new Vector3(0, 2, 0);
                                MeshRenderer meshRenderer;
                                meshRenderer = obj.GetComponent<MeshRenderer>();
                                meshRenderer.material = runnyWater;
                            }
                            Debug.Log("Water goes higher");
                        }
                    }
                    isRaining = true;
                    isDry = false;
                    isSnowing = false;
                    plank.gameObject.SetActive(true);
                    wetRiver.gameObject.SetActive(true);
                    iceRiver.gameObject.SetActive(false);
                    Debug.Log("It's raining!");
                }
                else if (interactedObject.tag == "RuneDry")
                {
                    //if changing weather, check previous weather and adjust water level 
                    if (!isDry == true)
                    {
                        if (isRaining == true)
                        {
                            foreach (GameObject obj in waterObjects)
                            {
                                //set size of water objects
                                obj.gameObject.transform.localScale += new Vector3(0, -2, 0);
                                //set material of water objects
                                MeshRenderer meshRenderer;
                                meshRenderer = obj.GetComponent<MeshRenderer>();
                                meshRenderer.material = runnyWater;
                            }
                            Debug.Log("Water goes lower");
                        }
                        else if (isSnowing == true)
                        {
                            foreach (GameObject obj in waterObjects)
                            {
                                //set material of water objects
                                MeshRenderer meshRenderer;
                                meshRenderer = obj.GetComponent<MeshRenderer>();
                                meshRenderer.material = runnyWater;
                            }
                            Debug.Log("Water does not change size");
                        }
                    }
                    isRaining = false;
                    isDry = true;
                    isSnowing = false;
                    plank.gameObject.SetActive(false);
                    wetRiver.gameObject.SetActive(true);
                    iceRiver.gameObject.SetActive(false);
                    Debug.Log("It's dry!");
                }
                else if (interactedObject.tag == "RuneSnow")
                {
                    //if changing weather, check previous weather and adjust water level 
                    if (!isSnowing == true)
                    {
                        if(isRaining == true)
                        {
                            foreach (GameObject obj in waterObjects)
                            {
                                obj.gameObject.transform.localScale += new Vector3(0, -2, 0);
                                MeshRenderer meshRenderer;
                                meshRenderer = obj.GetComponent<MeshRenderer>();
                                meshRenderer.material = ice;
                            }
                            Debug.Log("Water goes lower");
                        }
                        else if(isDry == true)
                        {
                            Debug.Log("Water does not change size");
                        }
                    }
                    isRaining = false;
                    isDry = false;
                    isSnowing = true;
                    plank.gameObject.SetActive(false);
                    wetRiver.gameObject.SetActive(false);
                    iceRiver.gameObject.SetActive(true);
                    Debug.Log("It's snowing!");
                }
                else
                {
                    //audioSource.PlayOneShot(pop, 0.2F);
                    Debug.Log("No object!");
                }

                //windmill sequence
                if(interactedObject.tag == "Plug" && isSnowing)
                {
                    plugIn = true;
                    Debug.Log(plugIn);
                }
                else if(plugIn == true && isDry == true)
                {
                    fuzeCharged = true;
                }
                else if(interactedObject.tag == "Plug" && fuzeCharged == true)
                {
                    hasFuze = true;
                    Debug.Log(hasFuze);
                }

                //plant sequence of events
                if (interactedObject.tag == "Seeds")
                {
                    haveSeeds = true;
                    Debug.Log(haveSeeds);
                }
                else if(interactedObject.tag == "Soil" && haveSeeds == true)
                {
                    seedsPlanted = true;
                    Debug.Log(seedsPlanted);
                }
                else if (seedsPlanted == true && isRaining == true)
                {
                    plantGrown = true;
                    plant.gameObject.SetActive(true);
                    Debug.Log(plantGrown);
                }
                else if (interactedObject.tag == "Plant" && plantGrown == true)
                {
                    havePlant = true;
                    Debug.Log(havePlant);
                }
                else if (havePlant == true && interactedObject.tag == "Generator")
                {
                    generatorOn = true;
                    Debug.Log(generatorOn);
                }

                //puddle sequence
                if(interactedObject.tag == "Lever" && generatorOn == true && isDry)
                {
                    bridgeUp = true;
                    Debug.Log(bridgeUp);
                    bridge.gameObject.SetActive(true);
                }
            }
        }
    }
}
