using UnityEngine;

/// <summary>
/// 저장 파일에 기록할 값들의 묶음.
/// 필드를 추가/삭제하면, 저장/불러오기 코드도 함께 업데이트해야 한다.
/// </summary>
[System.Serializable]   // 직렬화 클래스. 클래스의 내용을 통째로 순서대로 저장을 할 수 있다.
public class SaveData
{
    public float masterVolume;
    public float bgmVolume;
    public float sfxVolume;
}
