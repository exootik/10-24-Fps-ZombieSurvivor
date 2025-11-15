using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInteraction : MonoBehaviour
{
    public Transform cameraTransform;
    public float interactDistance = 3.5f;
    public LayerMask interactLayerMask = ~0;
    public InputActionReference interactAction;
    public TMP_Text lookPromptText;

    private Shop currentShop;

    private void Update()
    {
        UpdateLook();
    }

    private void OnEnable()
    {
        interactAction?.action.Enable();
        if (interactAction != null) interactAction.action.performed += OnInteractPerformed;
    }

    private void OnDisable()
    {
        if (interactAction != null) interactAction.action.performed -= OnInteractPerformed;
        interactAction?.action.Disable();
    }

    private void UpdateLook()
    {
        if (lookPromptText != null) lookPromptText.gameObject.SetActive(false);

        if (cameraTransform == null)
        {
            cameraTransform = Camera.main?.transform;
            if (cameraTransform == null) return;
        }

        var ray = new Ray(cameraTransform.position, cameraTransform.forward);
        if (Physics.Raycast(ray, out var hit, interactDistance, interactLayerMask, QueryTriggerInteraction.Ignore))
        {
            var shop = hit.collider.GetComponent<Shop>();
            if (shop != null)
            {
                if (lookPromptText != null)
                {
                    lookPromptText.text = "Press E to open the shop";
                    lookPromptText.gameObject.SetActive(true);
                }

                currentShop = shop;
                return;
            }
        }

        currentShop = null;
    }

    private void OnInteractPerformed(InputAction.CallbackContext ctx)
    {
        Debug.Log("On appuie sur E sur le shop si il est vide");
        if (currentShop != null)
        {
            Debug.Log("Press E to open the shop ");
            currentShop.OpenShop();
        }
    }
}