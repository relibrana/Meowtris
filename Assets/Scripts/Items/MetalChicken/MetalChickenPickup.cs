using UnityEngine;

/// <summary>
/// Pollo metálico capsule reward (item catalog #9).
/// Referencia de feeling (diseño, sep 2026): Mario metálico. NO salta menos que
/// un pollo normal — salta igual o más — pero cae más rápido por su peso, sus
/// patadas empujan más y a él no lo empuja nada.
/// </summary>
public sealed class MetalChickenPickup : MonoBehaviour, IInstantItem
{
    [Header("Pollo metálico")]
    [SerializeField, Min(1f), Tooltip("Duración del estado. El doc pide vigilarla: la inmunidad anula 2 de los 3 verbos ofensivos.")]
    private float duration = 6f;

    [SerializeField, Range(0.5f, 2f), Tooltip("Multiplicador de la velocidad inicial de salto. 1 = mismo salto que un pollo normal; puede subir de 1 para compensar gravedad extra.")]
    private float jumpMultiplier = 1f;

    [SerializeField, Range(0.5f, 3f), Tooltip("Multiplicador de gravedad general (afecta subida Y bajada: recorta el salto). Dejar en 1 salvo que se quiera un salto más corto y seco.")]
    private float gravityMultiplier = 1f;

    [SerializeField, Range(1f, 4f), Tooltip("Multiplicador de gravedad SOLO en caída: es el que da el peso metálico sin tocar la altura del salto.")]
    private float fallGravityMultiplier = 1.8f;

    [SerializeField, Min(1f), Tooltip("Multiplicador de fuerza de la patada.")]
    private float kickMultiplier = 3f;

    [SerializeField, Tooltip("Anula el planeo: el tope de caída del planeo cancelaría el peso metálico.")]
    private bool disableGlide = true;

    [SerializeField, Tooltip("Tinte placeholder metálico del portador.")]
    private Color metalTint = new Color(0.62f, 0.66f, 0.72f, 1f);

    public void Apply(PlayerController player)
    {
        if (!player.TryGetComponent(out MetalChickenState state))
            state = player.gameObject.AddComponent<MetalChickenState>();

        state.Activate(
            duration,
            jumpMultiplier,
            gravityMultiplier,
            fallGravityMultiplier,
            kickMultiplier,
            disableGlide,
            metalTint);
    }
}
