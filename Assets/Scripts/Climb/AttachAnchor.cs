using UnityEngine;
using Obi;
using System.Net.Mail;
using System.Collections;
public class AttachAnchor : MonoBehaviour
{
    public ObiParticleAttachment ball;
    public ObiParticleAttachment ball2;
    public ObiRope obiRope;
    public GameObject Rope;
    private ObiActor actor;      // ��ǰ����/���϶�Ӧ�� ObiActor
    private ObiSolver solver;    // �� actor ʹ�õ� Solver

    public bool isFirst = false;
    public GameObject playerAnchor;

    public bool hasAttached = false;
   
    void Start()
    {
        // �ӵ�һ�� attachment ��ȡ actor �� solver
        actor = ball.GetComponentInParent<ObiActor>();
        solver = actor.solver;
        if (!isFirst)
        {
            playerAnchor = null;
        }
        // ��ʼ�󶨵�������ʡ�ԣ��༭����Ҳ��ֱ���趨��
        ball.target = ball.transform;
        ball2.target = ball2.transform;
        if (!isFirst)
        {
            StartCoroutine(InitializeRope());
        }

       
        //Rope.SetActive(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        // ����Ѿ��󶨹�һ�Σ���ֱ�ӷ���
        if (hasAttached || isFirst || !CharacterClimb.isClimbing )
        {

            return;
        }
                    

        if (!other.CompareTag("Player"))
            return;

        Rope.SetActive(true);
        obiRope.enabled = true;
        if (AnchorPoint.attachAnchor != null)
        {
            AnchorPoint.attachAnchor.ResetAnchorPoint();

           

        }

        Debug.Log("��һ�ν���ê�㣬���а󶨲�ͬ���˵�λ��");
        SnapAndAttach(ball, other.transform);
        SnapAndAttach(ball2, other.transform);
        AnchorPoint.attachAnchor = this;
        hasAttached = true;   // �������ɵ�һ�ΰ�



    }


    private void OnTriggerExit(Collider other)
    {
           

        if (!other.CompareTag("Player"))
            return;

        Debug.Log("�뿪ê�㣬���а󶨲�ͬ���˵�λ��");


        //obiRope.enabled = false; 
        //Rope.SetActive(false);
        //hasAttached = false;   // �������ɵ�һ�ΰ�
    }

    public void ResetAnchorPoint()
    {
       
        //obiRope.enabled = false;
        //SnapAndAttach(ball, ball.transform);
        //SnapAndAttach(ball2, ball2.transform);
        ball.target = ball.transform;
        ball2.target = ball2.transform;
        ResetRope(obiRope);
        //obiRope.enabled = false;
        //Rope.SetActive(false);
        hasAttached = false;
    }
    /// <summary>
    /// �ȡ��⿪��attachment������Ӧ����˲�Ƶ� targetPos��
    /// �ٰ� attachment ����ָ�� target �����á�
    /// </summary>
    public void SnapAndAttach(ObiParticleAttachment attachment, Transform newTarget)
    {
        // 1. ȡ����� attachment Ӱ���������
        var group = attachment.particleGroup;
        if (group == null || group.particleIndices.Count == 0) return;

        // 2. �Ƚ��� attachment ����� target�������ذ�ʱ�Զ������ӡ�������ԭλ
        attachment.enabled = false;
        attachment.target = null;

        // 3. �ҵ���һ�������� solver �е�����
        int blueprintIndex = group.particleIndices[0];              // ������ blueprint �е�����
        int solverIndex = actor.solverIndices[blueprintIndex];  // ��Ӧ�� solver.positions ���������

        // 4. �Ѹ����ӵ�λ��˲�Ƶ� newTarget ��λ�ã��������� �� solver �������꣩
        Vector3 worldPos = newTarget.position;
        Vector3 localPos = solver.transform.InverseTransformPoint(worldPos);
        solver.positions[solverIndex] = localPos;
        // ��ѡ�������ٶȣ���ֹ˲�ƺ���ֶ���
        solver.velocities[solverIndex] = Vector3.zero;

        // 5. ���°󶨵���Ŀ�겢���� attachment
        attachment.target = newTarget;
        attachment.enabled = true;
    }
    public void Attach()
    {
        // 1. ȡ����� attachment Ӱ���������
        if (playerAnchor!=null)
        {
            Rope.SetActive(true);
            obiRope.enabled = true;
            SnapAndAttach(ball, playerAnchor.transform);
            SnapAndAttach(ball2, playerAnchor.transform);
            isFirst = false;
            AnchorPoint.attachAnchor = this;
            hasAttached = true;
        }
    }




    IEnumerator InitializeRope()
    {
        // Start �еĳ�ʼ���߼�
        for (int i = 0; i < 10; i++)
        {
            yield return null; // �ȴ�һ֡
        }// �ȴ�һ֡

        obiRope.enabled = false;
        // ����һִ֡�е��߼�
    }
    void ResetRope(ObiRope rope)
    {
        if (rope.isLoaded)
        {
            Matrix4x4 l2sTransform = rope.actorLocalToSolverMatrix;

            for (int i = 0; i < rope.particleCount; ++i)
            {
                int solverIndex = rope.solverIndices[i];

                rope.solver.positions[solverIndex] = l2sTransform.MultiplyPoint3x4(rope.blueprint.positions[i]);
                rope.solver.velocities[solverIndex] = Vector3.zero;
            }
        }
    }


}
