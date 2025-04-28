using UnityEngine;
using Obi;

[RequireComponent(typeof(ObiRopeExtrudedRenderer))]
public class RopeUVFixer : MonoBehaviour
{
    // U ��ÿȦƽ�̼���
    public float uTiles = 4;
    // V ��ÿ��λ����ƽ�̼��Σ��ر� NormalizeV ʱ��Ч��
    public float vTilesPerUnit = 1;

    private ObiRopeExtrudedRenderer rend;

    void Awake()
    {
        rend = GetComponent<ObiRopeExtrudedRenderer>();

        // �ر� NormalizeV������ V ��ͻ����ʵ�ʳ���ƽ��
        rend.normalizeV = false;

        // ���� UVScale��
        //   x = uTiles��U ���ظ�������
        //   y = vTilesPerUnit��V ��ÿ��λ�����ظ�������
        rend.uvScale = new Vector2(uTiles, vTilesPerUnit);
    }

    // ���������ӳ��Ȼᶯ̬�仯��������Ҫ���� V ��̶������������ڰ󶨺��ÿ֡���ã�
    void Update()
    {
        // float length = rend.ropeActor.Length;
        // rend.UVScale = new Vector2(uTiles, vTilesPerUnit * length);
    }
}
