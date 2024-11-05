using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Doublsb.Dialog;

// 최대한 주석 달아보았으나, 혹시 이해가 안되는 코드 있다면 저(이다연)한테 물어보셔도 되고, 챗지피티에게 코드 주석 달아달라고 하고 설명해달라고 하면 잘 설명해줍니다!
public class TalkDialogue : MonoBehaviour // TalkDialogue는 튜토리얼 전체 대사가 들어있음, 대사 코드 뒤에 오브젝트 등장, 애니메이션 작동 모두 관리하고 있습니다.
{
    public DialogManager DialogManager; // DialogManager 스크립트를 참조해서 함수사용함.

    // 모든 animation들은 standing, fast, move 가 trigger로 animator에서 전환 가능하게 만듦, 기본 애니메이션은 Idle로 설정함.

    public Animator noonDungAnimator;  // NoonDung 오브젝트의 Animator 
    public Animator snowflakeAnimator1; // snowflake1 오브젝트의 Animator
    public Animator snowflakeAnimator2; // snowflake2 오브젝트의 Animator
    public Animator snowflakeAnimator3; // snowflake3 오브젝트의 Animator
    public Animator roroAnimator;      // roro 오브젝트의 Animator
    public Animator noonkyeolAnimator; // noonkyeol 오브젝트의 Animator
    public Animator kkotsongAnimator;  // kkotsong 오브젝트의 Animator
    public Animator noonsongAnimator;  // noonsong 오브젝트의 Animator
    public Animator turiAnimator;      // turi 오브젝트의 Animator

    public GameObject noonDung;  // part1 눈덩이 등장
    public GameObject snowflake; // part2 눈꽃송이 등장
    public GameObject roro;      // part3 로로 등장
    public GameObject noonkyeol; // part4 눈결이 등장
    public GameObject kkotsong;  // part5 꽃송이 등장
    public GameObject noonsong;  // part6 눈송이 등장
    public GameObject turi;      // part7 튜리 등장

    public GameObject ParticlePanel; // ParticlePanel 오브젝트
    public GameObject Count;
    public GameObject StudentIdPanel;
    public GameObject StudentId;

    public Transform arCamera; // AR 카메라 Transform
    public float moveDuration = 2f; // 이동 애니메이션 지속 시간

    public bool[] dialogTriggered = new bool[4]; // 다이얼로그가 호출되었는지 여부를 저장

    // 효과음 오디오 클립
    public AudioClip noonDungSound;
    public AudioClip snowflakeSound;
    public AudioClip roroSound;
    public AudioClip noonkyeolSound;
    public AudioClip kkotsongSound;
    public AudioClip noonsongSound;
    public AudioClip turiSound;

    // 각 오브젝트에 대한 사운드 매핑
    private Dictionary<GameObject, AudioClip> objectSoundMap;


    // 첫 번째 대화 설정 1~3
    public void FirstDialog()
    {
        if (dialogTriggered[0]) return;

        var FirstDialog = new List<DialogData>();

        // 오브젝트 등장 위치 조정하고 싶으시다면 여기다가 해당 등장 오브젝트의 MoveObject 들어간 코드를 넣어서 사용하시면 제일 먼저 나온답니다.
        // example : FistDialog.Add(new DialogData("/color:black//emote:Happy/찾았다, 눈송이!", "KkotSong", () => { noonsong.SetActive(true); StartCoroutine(MoveObject(noonsong, arCamera.TransformPoint(new Vector3(-1.5f, -0.3f, 5f)), arCamera.TransformPoint(new Vector3(0f, -0.3f, 2f)))); })); // 화면 좌측에서 등장 눈송이 시작

        // part1 눈덩이 등장
        FirstDialog.Add(new DialogData("/color:black/숙명여대에 갓 입학한 새송이는 학교 탐방을 오게 되었다!", "Narrator"));
        FirstDialog.Add(new DialogData("/color:black/그런데 어쩌지? 학교가 너무 복잡해!","Narrator"));
        FirstDialog.Add(new DialogData("/color:black/[학교가 너무 처음이라 막막하네...]", "User", () => { noonDung.SetActive(true); StartCoroutine(MoveObject(noonDung, arCamera.TransformPoint(new Vector3(-1.5f, 1.5f, 5f)), arCamera.TransformPoint(new Vector3(0f, 0f, 1.5f)))); })); // 좌측 위 등장 눈덩이 시작)))));
        FirstDialog.Add(new DialogData("/color:black//wait:0.5/안녕, 친구야! 혹시 무슨 고민 있어?", "NoonDung"));
        FirstDialog.Add(new DialogData("/color:black/[(사정을 설명한다.)]", "User", () => ChangeAnimation(noonDungAnimator, "standing"))); // 원하는 애니메이션은 바로 전 대사 뒤에 붙여야 자연스레 이어짐.
        FirstDialog.Add(new DialogData("/color:black/아하, 아직 학교가 처음이라 모르는 게 많다고? 음.. 어디보자~", "NoonDung"));
        FirstDialog.Add(new DialogData("/color:black/그렇지! 숙명여대라면 역시 눈송이! 그 애가 널 도와줄 수 있을 거야!", "NoonDung"));
        FirstDialog.Add(new DialogData("/color:black/같이 학교를 돌아다니면서 /color:blue/눈송이/color:black/가 어디에 있는지 찾아보자!/wait:1//close/", "NoonDung", () => noonDung.SetActive(false)));
        // part2 눈꽃송이 등장
        FirstDialog.Add(new DialogData("/color:black//wait:1/[저기 하늘에 떠 다니는 건 뭐지?]", "User"));
        FirstDialog.Add(new DialogData("/color:black/어디? 어디?", "NoonDung", () => { snowflake.SetActive(true); StartCoroutine(MoveObject(snowflake, arCamera.TransformPoint(new Vector3(0.35f, 2f, -1f)), arCamera.TransformPoint(new Vector3(0.35f, 0f, -1f)))); })); // 하늘에서 내려옴 눈꽃송이 시작
        FirstDialog.Add(new DialogData("/color:black//wait:1/우리는! /wait:0.5/학교를 지키는 어벤져스, /click/눈꽃송이들이야!", "Snowflake"));
        FirstDialog.Add(new DialogData("/color:black/마침 잘 만났다! 얘들아, 이 새송이가 눈송이와 친구가 되고 싶대!", "NoonDung"));
        FirstDialog.Add(new DialogData("/color:black/그런거라면... 눈송이는 눈의 결정을 좋아하니까, 눈의 결정을 준다면 분명 친구가 될 수 있을 거야", "Snowflake", () => StartCoroutine(ShowPanelFirst())));
        FirstDialog.Add(new DialogData("/color:black//wait:0.5/마침 우리한테 꿍쳐놓은 눈의 결정이 있으니까, 너한테 줄게!", "Snowflake", () => ChangeAnimation(snowflakeAnimator1, "standing")));
        FirstDialog.Add(new DialogData("/color:black/어려운 친구를 돕는 것도 우리 일이니까. 우리가 새송이를 도와주는 건 어떨까? /click/(뭉치면 산다!)", "Snowflake", () => ChangeAnimation(snowflakeAnimator1, "standing")));
        FirstDialog.Add(new DialogData("/color:black/그래, 눈의 결정이라면 우리가 전문이니까, 함께 다니면서 눈의 결정 찾는걸 도와줄게! /click/(맡겨 줘!)", "Snowflake"));
        FirstDialog.Add(new DialogData("/color:black/[고마워, 눈꽃송이들!]/wait:1//close/", "User", () => snowflake.SetActive(false)));
        // part3 로로 등장
        FirstDialog.Add(new DialogData("/color:black//wait:1/앗! 찾았다", "RoRo", () => { roro.SetActive(true); StartCoroutine(MoveObject(roro, arCamera.TransformPoint(new Vector3(0f, 1f, 5f)), arCamera.TransformPoint(new Vector3(0f, -0.2f, 1f)))); })); // 멀리서 달려오듯이 등장 로로 시작
        FirstDialog.Add(new DialogData("/color:black/[앗!]", "User", () => ChangeAnimation(roroAnimator, "standing")));
        FirstDialog.Add(new DialogData("/color:black/네가 눈송이를 찾아 다닌다는 새송이 맞지! 소식을 듣고 한달음에 달려왔어!", "RoRo", () => ChangeAnimation(noonDungAnimator, "standing")));
        FirstDialog.Add(new DialogData("/color:black/로로잖아! 과연 학교의 소식통이라 그런지, 소식이 빠르네!", "NoonDung", () => ChangeAnimation(roroAnimator, "standing")));
        FirstDialog.Add(new DialogData("/color:black//emote:Happy/히히, 1캠퍼스 정문에서 눈송이를 본 것 같다는 걸 알려주려고! 새송이가 누군지 궁금하기도 했고!", "RoRo"));
        FirstDialog.Add(new DialogData("/color:black//emote:Call/아직 학교 지리는 잘 모르지? 내가 같이 가줄게!", "RoRo", () => {roro.SetActive(false); dialogTriggered[0] = true;}));
        
        DialogManager.Show(FirstDialog);
    }

    // 4
    public void SecondDialog()
    {
        if (dialogTriggered[1]) return;
        if (!dialogTriggered[0]) return; // 이전 다이얼로그가 호출되지 않았으면 return

        

        var SecondDialog = new List<DialogData>();

        // part4 눈결이 등장
        SecondDialog.Add(new DialogData("/color:black//emote:Hello/저기,, 안녕하세요! 처음 보는 분이네요..!", "NoonGyeol", () => { noonkyeol.SetActive(true); StartCoroutine(MoveObject(noonkyeol, arCamera.TransformPoint(new Vector3(2f, -0.5f, 5f)), arCamera.TransformPoint(new Vector3(0f, -0.5f, 3f)))); })); // 오른쪽에서 천천히 등장 눈결이 시작));
        SecondDialog.Add(new DialogData("/color:black/눈결이 안녕! 혹시 근처에서 눈송이 못봤어?", "NoonDung", () => ChangeAnimation(noonkyeolAnimator, "standing")));
        SecondDialog.Add(new DialogData("/color:black/눈송이 말인가요? 음... 못 봤어요. 무슨 일이신데요?", "NoonGyeol"));
        SecondDialog.Add(new DialogData("/color:black/[(눈결이에게 사정을 설명한다)]", "User"));
        SecondDialog.Add(new DialogData("/color:black//emote:Look/앗, 그렇다면 이게 도움이 될 거예요!", "NoonGyeol", () => ChangeAnimation(noonkyeolAnimator, "standing")));
        SecondDialog.Add(new DialogData("/color:black//emote:Study/저는 지도를 잘 보거든요. 그 외에 이것저것 많은 것을 알고 있으니까, 제 지식이 도움이 될 수 있을 것 같아요.", "NoonGyeol"));
        SecondDialog.Add(new DialogData("/color:black/[(그럼 혹시 도와줄 수 있냐고 묻는다.)]", "User"));
        SecondDialog.Add(new DialogData("/color:black/물론이에요..! 저도 동행할게요.", "NoonGyeol"));
        SecondDialog.Add(new DialogData("/color:black/으~음.. 눈송이 대신 눈결이가 있었네. 괜찮아! 마침 한 곳 더 짐작이 가는 곳이 있어!", "RoRo"));
        SecondDialog.Add(new DialogData("/color:black/2캠퍼스 정문으로 가보자!", "RoRo", () => {noonkyeol.SetActive(false); dialogTriggered[1] = true;}));

        DialogManager.Show(SecondDialog);
    }

    // 5
    public void ThirdDialog()
    {
        if (dialogTriggered[2]) return;
        if (!dialogTriggered[1]) return; // 이전 다이얼로그가 호출되지 않았으면 return


        var ThirdDialog = new List<DialogData>();

        // part5 꽃송이 등장
        ThirdDialog.Add(new DialogData("/color:black/안녕, 친구들? /click//emote:Excite/처음 보는 친구도 있구나! 새송이인가 보네?", "KkotSong", () => { kkotsong.SetActive(true); StartCoroutine(MoveObject(kkotsong, arCamera.TransformPoint(new Vector3(0f, -0.3f, 2f)), arCamera.TransformPoint(new Vector3(0f, -0.3f, 2f)))); })); // 화면 가운데서 춤 연습중 꽃송이 시작
        ThirdDialog.Add(new DialogData("/color:black//emote:Hello/이 친구는 꽃송이야! 눈송이의 베프인 꽃송이라면 눈송이가 어디 있는지 알 지도 몰라!", "RoRo", () => ChangeAnimation(kkotsongAnimator, "standing")));
        ThirdDialog.Add(new DialogData("/color:black/눈송이? 너희 눈송이를 찾고 있니?", "KkotSong"));
        ThirdDialog.Add(new DialogData("/color:black/맞아요. 새송이가 눈송이와 친구가 되고 싶대요.", "RoRo"));
        ThirdDialog.Add(new DialogData("/color:black//emote:Happy/그렇다면 정확히 찾아 왔어. 마침 방금 전까지 눈송이랑 함께 있던 참이었거든.", "KkotSong", () => ChangeAnimation(kkotsongAnimator, "standing")));
        ThirdDialog.Add(new DialogData("/color:black/아마 눈송이는 /color:blue/프라임관/color:black/에 있을 거야! 어딘지 아니? 같이 가 줄게.", "KkotSong", () => {kkotsong.SetActive(false); dialogTriggered[2] = true;}));

        DialogManager.Show(ThirdDialog);
    }

    // 6~7
    public void FourthDialog()
    {
        if (dialogTriggered[3]) return;
        if (!dialogTriggered[2]) return; // 이전 다이얼로그가 호출되지 않았으면 return

        dialogTriggered[3] = true;

        var FourthDialog = new List<DialogData>();

        // part 6,7

        // 눈송이 등장
        FourthDialog.Add(new DialogData("/color:black//emote:Happy/찾았다, 눈송이!", "KkotSong", () => { noonsong.SetActive(true); StartCoroutine(MoveObject(noonsong, arCamera.TransformPoint(new Vector3(-1.5f, -0.3f, 5f)), arCamera.TransformPoint(new Vector3(0f, -0.3f, 2f)))); })); // 화면 좌측에서 등장 눈송이 시작
        FourthDialog.Add(new DialogData("/color:black//emote:Excite/안녕, 친구들! 어라, 처음 보는 친구도 있네?", "NoonSong"));
        FourthDialog.Add(new DialogData("/color:black/이 애가 너와 친구가 되고 싶다고 해서 데려왔어!", "NoonDung"));
        FourthDialog.Add(new DialogData("/color:black/눈송이를 위한 선물도 가져왔어! (두근두근…!)", "Snowflake", () => ChangeAnimation(noonsongAnimator, "standing")));
        FourthDialog.Add(new DialogData("/color:black//emote:Love/와아, 눈의 결정이네! 정말 기뻐!", "NoonSong"));
        FourthDialog.Add(new DialogData("/color:black/이렇게 찾아와 줘서 고마워, 그럼 우리 오늘부터 친구 하자!", "NoonSong"));
        FourthDialog.Add(new DialogData("/color:black/[(눈송이와 친구가 되자.)]/wait:2.0/", "User", () => noonsong.SetActive(false)));

        // 튜리 등장
        FourthDialog.Add(new DialogData("/color:black/앗-! 다들 나만 빼고 여기 모여 있었구나!", "Turi", () => { turi.SetActive(true); StartCoroutine(MoveObject(turi, arCamera.TransformPoint(new Vector3(0f, -2f, 3f)), arCamera.TransformPoint(new Vector3(0f, 0f, 3f)))); })); // 아래에서 등장 튜리 시작
        FourthDialog.Add(new DialogData("/color:black/어라? 못 보던 얼굴도 있네?", "Turi"));
        FourthDialog.Add(new DialogData("/color:black/[(인사한다.)]", "Turi", () => ChangeAnimation(turiAnimator, "standing")));
        FourthDialog.Add(new DialogData("/color:black/안녕, 튜리! 이 애는 새로운 눈송이인데, 나랑 친구가 되고 싶다고 찾아와 줬어!", "NoonSong"));
        FourthDialog.Add(new DialogData("/color:black/오… 이해했어!", "Turi"));
        FourthDialog.Add(new DialogData("/color:black/거기 눈송, 좀 더 다양한 눈송이를 만나보고 싶지 않아?", "Turi"));
        FourthDialog.Add(new DialogData("/color:black/[(고개를 끄덕인다.)]", "User", () => ChangeAnimation(turiAnimator, "standing")));
        FourthDialog.Add(new DialogData("/color:black/흐흥, 미래 산업시대의 선구자가 될 이 튜리님에게 너 같은 친구들을 위한 발명품이 있지!", "Turi"));
        FourthDialog.Add(new DialogData("/color:black/바로 /color:blue/특별한 눈의 결정/color:black/! 특별한 눈송이들의 마음도 사로잡을 수 있는 대단한 아이템이야!", "Turi", () => ChangeAnimation(turiAnimator, "standing")));
        FourthDialog.Add(new DialogData("/color:black/어때? 가지고 싶지? 그 대신, 특별한 눈의 결정을 만들기 위해서는 눈의 결정이 많이 필요해.", "Turi"));
        FourthDialog.Add(new DialogData("/color:blue/제2캠퍼스 원형 광장/color:black/에 있는 내 연구실에 오면 다양한 특별한 눈의 결정으로 바꿔줄게! 잊지 말고 놀러 와~!", "Turi", () => turi.SetActive(false)));
        FourthDialog.Add(new DialogData("/color:black/튜토리얼을 성공적으로 마친 당신에게 이것을 드립니다!", "Narrator", () => StartCoroutine(ShowPanelSecond())));
        FourthDialog.Add(new DialogData("/color:black//wait:0.5/다양한 눈송이들과 친구가 되어 완벽한 학생증을 완성하는 것을 목표로 힘을 내 봐요!", "Narrator"));
        FourthDialog.Add(new DialogData("/color:black/도감을 다 채우고 학생증을 완성한다면, 특별한 선물이 있을 지도…!?", "Narrator", () => StartCoroutine(ShowPanel3())));
        DialogManager.Show(FourthDialog);
    }


    // 다이얼로그가 호출되었는지 여부를 반환 (차례대로 호출되어야 함)
    public bool IsDialogTriggered(int index)
    {
        if (index >= 0 && index < dialogTriggered.Length)
        {
            return dialogTriggered[index];
        }
        return false;
    }


    private void Awake()
    {
        // 오브젝트와 사운드를 매핑
        objectSoundMap = new Dictionary<GameObject, AudioClip>
        {
            { noonDung, noonDungSound },
            { snowflake, snowflakeSound },
            { roro, roroSound },
            { noonkyeol, noonkyeolSound },
            { kkotsong, kkotsongSound },
            { noonsong, noonsongSound },
            { turi, turiSound }
        };

        FirstDialog();
    }

    // 애니메이션 변경 함수 (애니메이터 이름, trigger 문자열) 받음
    private void ChangeAnimation(Animator animator, string trigger)
    {
        animator.SetTrigger(trigger); // 애니메이션 트리거 설정

        if (animator == snowflakeAnimator1)
        {
            snowflakeAnimator2.SetTrigger(trigger);
            snowflakeAnimator3.SetTrigger(trigger);
        }
    }

    // 3D 오브젝트가 카메라 좌표계를 사용해서 움직일 수 있게하는 함수 (오브젝트, 시작 위치, 도착 위치)
    private IEnumerator MoveObject(GameObject obj, Vector3 startPos, Vector3 endPos)
    {
        // 초기 위치 설정
        obj.transform.position = startPos;

        // 오브젝트에 할당된 사운드가 있는지 확인하고 재생
        if (objectSoundMap.TryGetValue(obj, out AudioClip assignedSound))
        {
            AudioSource.PlayClipAtPoint(assignedSound, obj.transform.position);
        }

        // 이동 애니메이션 실행
        float elapsedTime = 0f;
        while (elapsedTime < moveDuration)
        {
            elapsedTime += Time.deltaTime;
            float t = Mathf.Clamp01(elapsedTime / moveDuration);
            obj.transform.position = Vector3.Lerp(startPos, endPos, t);
            yield return null;
        }

        // 정확한 최종 위치로 설정
        obj.transform.position = endPos;
    }

    private IEnumerator ShowPanelFirst()
    {
        Time.timeScale = 0f; // 시간 정지
                             // 패널 활성화
        ParticlePanel.SetActive(true);

        while (!Input.GetMouseButtonDown(0)) // 마우스 클릭을 기다림
        {
            yield return null; // 한 프레임을 대기
        }
        Count.SetActive(true);
        Time.timeScale = 1f; // 시간 재개
    }

    private IEnumerator ShowPanelSecond()
    {
        Time.timeScale = 0f; // 시간 정지
                             // 패널 활성화
        StudentIdPanel.SetActive(true);

        while (!Input.GetMouseButtonDown(0)) // 마우스 클릭을 기다림
        {
            yield return null; // 한 프레임을 대기
        }

        Time.timeScale = 1f; // 시간 재개
    }

    private IEnumerator ShowPanel3()
    {
        StudentId.SetActive(true);
        yield return null;
    }
}