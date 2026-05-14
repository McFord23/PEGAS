using Unity.Netcode.Components;

/**
 * Позволяет отправлять клиенту запросы на синхронизацию трансформа
 */
public class ClientNetworkTransform : NetworkTransform
{
    protected override bool OnIsServerAuthoritative()
    {
        return false;
    }
}
