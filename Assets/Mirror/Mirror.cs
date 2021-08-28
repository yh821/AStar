using UnityEngine;

/// <summary>
/// ���ӹ���ű� ���� �����½���Camera��
/// </summary>
[ExecuteInEditMode]
public class Mirror : MonoBehaviour
{
    public GameObject mirrorPlane;  //����
    public Camera mainCamera;   //�������
    private Camera mirrorCamera; //���������


    private void Start()
    {
        mirrorCamera = GetComponent<Camera>();
    }

    private void Update()
    {
        if (null == mirrorPlane || null == mirrorCamera || null == mainCamera) return;
        var positionInMirrorSpace = mirrorPlane.transform.InverseTransformPoint(mainCamera.transform.position); //�������������������λ��ת��Ϊ���ӵľֲ�����λ��
        positionInMirrorSpace.y = -positionInMirrorSpace.y;                                                    //һ��yΪ����ķ��߷���
        mirrorCamera.transform.position = mirrorPlane.transform.TransformPoint(positionInMirrorSpace);                 //ת�ص���������ϵ��λ��
    }
}