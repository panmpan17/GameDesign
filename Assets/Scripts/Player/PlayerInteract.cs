using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInteract : MonoBehaviour
{
    [Header("Reference")]
    [SerializeField]
    private PlayerBehaviour behaviour;
    [SerializeField]
    private PlayerMovement movement;
    [SerializeField]
    private Inventory inventory;
    [SerializeField]
    private EventReference canInteractEvent;
    [SerializeField]
    private EventReference dialogueEndEvent;

    [Header("Setting")]
    [SerializeField]
    private LayerMask interactLayer;
    [SerializeField]
    private float interactRaycastDistance;

    private GameObject _interactObject;
    private bool HasInteractObject => _interactObject != null;
    private Collider[] _interactColliders = new Collider[1];

    void Awake()
    {
        dialogueEndEvent.InvokeEvents += DialogueUIEnd;
        GetComponent<InputInterface>().OnInteract += OnInteract;
    }

    void Update()
    {
        UpdateInteractCheck();
    }

    void UpdateInteractCheck()
    {
        bool isOverlap = Physics.OverlapSphereNonAlloc(transform.position, interactRaycastDistance, _interactColliders, interactLayer) > 0;

        if (isOverlap)
        {
            if (!HasInteractObject)
                CheckGameObjectIsRightClickInteractable(_interactColliders[0].gameObject);
        }
        else
        {
            if (HasInteractObject)
            {
                _interactObject = null;
                canInteractEvent.Invoke(false);
            }
        }
    }

    void CheckGameObjectIsRightClickInteractable(GameObject interactGameObject)
    {
        if (!interactGameObject.CompareTag("RightClickInteractive"))
            return;

        _interactObject = interactGameObject;
        canInteractEvent.Invoke(true);
    }

    public void OnInteract()
    {
        if (!behaviour.CursorFocued)
            return;
        if (!_interactObject)
            return;

        if (_interactObject.GetComponent<NPCControl>() is var npc && npc)
        {
            npc.StartDialogue();

            canInteractEvent?.Invoke(false);
            movement.FaceRotationWithoutRotateFollowTarget(npc.transform.position);
            return;
        }

        else if (_interactObject.GetComponent<TreasureChest>() is var chest && chest)
        {
            chest.Open();
            canInteractEvent?.Invoke(false);
            movement.FaceRotationWithoutRotateFollowTarget(chest.transform.position);
        }

        else if (_interactObject.GetComponent<PortalGate>() is var portal && portal)
        {
            behaviour.InstantTeleportTo(portal.Teleport());
            canInteractEvent?.Invoke(false);
        }

        else if (_interactObject.GetComponent<PickupFlower>() is var flower && flower)
        {
            flower.Pickup();
            inventory.FlowerEvent.Invoke(true);
        }
    }


    void DialogueUIEnd()
    {
        if (HasInteractObject)
        {
            canInteractEvent.Invoke(true);
        }
    }


    void OnDestroy()
    {
        dialogueEndEvent.InvokeEvents -= DialogueUIEnd;
    }
}
