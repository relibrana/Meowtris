using UnityEngine;

/// <summary>
/// POW (item catalog #2). Cambio de diseño (sep 2026): ya no se activa al romper
/// la cápsula. Se lleva en la mano como el moco o la llanta, se lanza con el
/// botón de colocar y **la cuenta atrás arranca en el impacto** — así el jugador
/// elige el momento y el sitio en vez de dispararse solo al recoger.
/// El aturdimiento en sí lo resuelve PowSequence, igual que antes.
/// </summary>
public sealed class PowPickup : ThrowableItem
{
    [Header("POW")]
    [SerializeField, Range(1, 5), Tooltip("Desde dónde cuenta la cuenta regresiva en pantalla.")]
    private int countFrom = 3;

    [SerializeField, Min(0.5f), Tooltip("Duración del aturdimiento. Ojo: la cámara sigue subiendo (doc).")]
    private float stunSeconds = 2f;

    [SerializeField, Tooltip("Tinte placeholder de los jugadores aturdidos.")]
    private Color stunTint = new Color(0.6f, 0.6f, 0.75f, 1f);

    /// <summary>Primer impacto tras el lanzamiento: arranca el contador y se consume.</summary>
    protected override void OnProjectileHit(Collision2D collision)
    {
        PowSequence.Run(countFrom, stunSeconds, stunTint);
        gameObject.SetActive(false);
    }
}
