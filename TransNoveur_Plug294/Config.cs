using UnityEngine;

namespace TransNoveur_Plug294;

public class Config
{
    public int bizId = 0;
    public string discordWebhook = "";
    public float pointX = 262.8445f;
    public float pointY = 50.21f;
    public float pointZ = 992.6866f;
    public float spawnX = 269.4228f;
    public float spawnY = 50.26678f;
    public float spawnZ = 992.7704f;
    public float spawnRotX = -7.113395E-05f;
    public float spawnRotY = 190.9756f;
    public float spawnRotZ = -4.949392E-05f;
    public float menuX = 0f;
    public float menuY = 0f;
    public float menuZ = 0f;

    public Vector3 GetPointPosition() => new UnityEngine.Vector3(pointX, pointY, pointZ);
    public Vector3 GetPointMenuPosition() => new UnityEngine.Vector3(menuX, menuY, menuZ);
    public Vector3 GetSpawnPosition() => new UnityEngine.Vector3(spawnX, spawnY, spawnZ);
    public Quaternion GetSpawnRotation() => Quaternion.Euler(0f, spawnRotY, 0f);
}