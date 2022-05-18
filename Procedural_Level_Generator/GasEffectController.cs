using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using cakeslice;
using static Prompts;

public class GasEffectController : MonoBehaviourPun
{

    public GameManager GM;
    public GasEffectController otherNexus;
    public GameObject[] cubes;
    
    public GameObject gasEffect = null;
    public bool active = false;

    private void Start()
    {
        GM = FindObjectOfType<GameManager>();

    }
    private void OnTriggerEnter(Collider other)
    {
        Scientist s = other.gameObject.GetComponent<Scientist>();
        if (s != null && s.isAlive && s.photonView.IsMine && s.inventory.Fetch("Keycard") != null)
        {
            s.TogglePrompt(PromptString.nexus, true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        Scientist s = other.gameObject.GetComponent<Scientist>();
        if (s != null && s.isAlive && s.photonView.IsMine)
        {
            s.TogglePrompt(PromptString.blank, false);
        }
    }
    private void OnTriggerStay(Collider other)
    {

        Scientist s = other.gameObject.GetComponent<Scientist>();
        if (!active && s != null && s.isAlive && s.photonView.IsMine && s.inventory.Fetch("Keycard") != null)
        {
            if (Input.GetButtonDown("Interact"))
            {
                s.inventory.Remove(s.inventory.Fetch("Keycard"));
                s.TogglePrompt(PromptString.blank, false);
                this.photonView.RPC("ActivateNexus", RpcTarget.All);
                s.AM.PlayAllClipAtSpot("Keycard", transform.position);
            }
            else
            {
                s.TogglePrompt(PromptString.nexus, true);
            }
        }
    }

    [PunRPC]
    public void ActivateNexus() {
        active = true;
        foreach (GameObject cube in cubes)//put this in the start method with a check to see if the monster is mine to not allow monsters to see change
        {
            cube.GetComponent<Outline>().color = 0;
        }
        if (otherNexus.active)
        {
            EnableEffect();
        }
    }

    private void EnableEffect()
    {
        gasEffect.SetActive(true);
        GM.gasInRoom = true;
    }
}
