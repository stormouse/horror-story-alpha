using UnityEngine;

public interface ISensible {
    void Activate();
    void See(GameObject obj);
    void LoseSight(GameObject obj);
    void Hear(GameObject obj);
    void LoseHearing(GameObject obj);
}
