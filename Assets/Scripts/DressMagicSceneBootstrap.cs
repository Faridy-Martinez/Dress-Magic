using UnityEngine;

/// <summary>
/// Añade este componente a un GameObject vacío de la escena inicial.
/// El juego construye su interfaz automáticamente al iniciar Play.
/// </summary>
public class DressMagicSceneBootstrap : MonoBehaviour
{
    private void Awake()
    {
        if (FindObjectOfType<DressMagicGame>() == null)
            gameObject.AddComponent<DressMagicGame>();
    }
}