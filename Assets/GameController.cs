using UnityEngine;

public static class GameController
{

    private static int collectableCount;
    public static float tempo;
    public static bool perdeu;
    public static int vidas;

    public static int coletados{
        get { return 3 - collectableCount; }
    }

    public static bool venceu{
        get { return collectableCount <= 0; }
    }

    public static bool gameOver{
        get { return venceu || perdeu; }
    }

    public static void Init(){
        collectableCount = 3;
        tempo = 60f;
        perdeu = false;
        vidas = 3;
    }

    public static void Collect(){
        collectableCount--;
    }

    public static void LevarDano(){
        vidas--;
        if (vidas <= 0){
            vidas = 0;
            perdeu = true;
        }
    }
    
}