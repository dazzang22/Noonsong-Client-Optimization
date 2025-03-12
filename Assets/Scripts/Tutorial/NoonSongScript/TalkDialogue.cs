using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Doublsb.Dialog;
using UnityEngine.SceneManagement;
using UltimateClean;

// 최대한 주석 달아보았으나, 혹시 이해가 안되는 코드 있다면 저(이다연)한테 물어보셔도 되고, 챗지피티에게 코드 주석 달아달라고 하고 설명해달라고 하면 잘 설명해줍니다!
public class TalkDialogue : MonoBehaviour // TalkDialogue는 튜토리얼 전체 대사가 들어있음, 대사 코드 뒤에 오브젝트 등장, 애니메이션 작동 모두 관리하고 있습니다.
{
    public DialogManager DialogManager; // DialogManager 스크립트를 참조해서 함수사용함.
    public UIController uiController;

    // 모든 animation들은 standing, fast, move 가 trigger로 animator에서 전환 가능하게 만듦, 기본 애니메이션은 Idle로 설정함.
    [Header("Animator")]
    public Animator noonDungAnimator;  // NoonDung 오브젝트의 Animator 
    public Animator snowflakeAnimator1; // snowflake1 오브젝트의 Animator
    public Animator snowflakeAnimator2; // snowflake2 오브젝트의 Animator
    public Animator snowflakeAnimator3; // snowflake3 오브젝트의 Animator
    public Animator roroAnimator;      // roro 오브젝트의 Animator
    public Animator noonkyeolAnimator; // noonkyeol 오브젝트의 Animator
    public Animator kkotsongAnimator;  // kkotsong 오브젝트의 Animator
    public Animator noonsongAnimator;  // noonsong 오브젝트의 Animator
    public Animator turiAnimator;      // turi 오브젝트의 Animator

    [Header("Prefab")]
    public GameObject noonDung;  // part1 눈덩이 등장
    public GameObject snowflake; // part2 눈꽃송이 등장
    public GameObject roro;      // part3 로로 등장
    public GameObject noonkyeol; // part4 눈결이 등장
    public GameObject kkotsong;  // part5 꽃송이 등장
    public GameObject noonsong;  // part6 눈송이 등장
    public GameObject turi;      // part7 튜리 등장
    public GameObject map;
    public GameObject profile;
    public GameObject goods;


    [Header("AudioClip")]
    // 효과음 오디오 클립
    public AudioClip noonDungSound;
    public AudioClip snowflakeSound;
    public AudioClip roroSound;
    public AudioClip noonkyeolSound;
    public AudioClip kkotsongSound;
    public AudioClip noonsongSound;
    public AudioClip turiSound;
    public AudioClip getMapSound;
    public AudioClip getGoodsSound;
    public AudioClip getProfileSound;

    [Header("")]
    public GameObject Count;
    public Text printerText;
    // public GameObject mainCanvas;
    // public GameObject mainPanel;
    public Canvas mapCanvas;
    public Canvas giftCanvas;
    public Canvas bookCanvas;
    public GameObject StudentId;

    public Transform arCamera; // AR 카메라 Transform
    public float moveDuration = 2f; // 이동 애니메이션 지속 시간

    public bool[] dialogTriggered = new bool[4]; // 다이얼로그가 호출되었는지 여부를 저장

    // 각 오브젝트에 대한 사운드 매핑
    private Dictionary<GameObject, AudioClip> objectSoundMap;

    [Header("Gift")]
    // 선물 반응 이펙트
    public GameObject effectObject;
    // 선물 주기 팝업 UI
    public GameObject popupUI;
    public Button slotButton; // 슬롯 버튼 (선물 버튼)
    public Button giftButton; // 팝업의 "Yes" 버튼
    public Button cancelButton; // 팝업의 "No" 버튼

    // 첫 번째 대화 설정 1~3
    public void FirstDialog()
    {
        Debug.Log("시작");

        if (dialogTriggered[0]) return;

        var FirstDialog = new List<DialogData>();

        // 오브젝트 등장 위치 조정하고 싶으시다면 여기다가 해당 등장 오브젝트의 MoveObject 들어간 코드를 넣어서 사용하시면 제일 먼저 나온답니다.
        // example : FistDialog.Add(new DialogData("/color:black//emote:Happy/찾았다, 눈송이!", "KkotSong", () => { noonsong.SetActive(true); StartCoroutine(MoveObject(noonsong, arCamera.TransformPoint(new Vector3(-1.5f, -0.3f, 5f)), arCamera.TransformPoint(new Vector3(0f, -0.3f, 2f)))); })); // 화면 좌측에서 등장 눈송이 시작

        // part1 눈덩이 등장
        FirstDialog.Add(new DialogData("/color:black/숙명여대에 갓 입학한 새송이는 학교 탐방을 오게 되었다!", "Narrator"));
        FirstDialog.Add(new DialogData("/color:black/그런데 어쩌지? 학교가 너무 복잡해!","Narrator"));
        FirstDialog.Add(new DialogData("/color:black/[학교가 너무 처음이라 막막하네...]", "User", () => { noonDung.SetActive(true); PlaySound(noonDung);
        //StartCoroutine(MoveObject(noonDung, arCamera.TransformPoint(new Vector3(-1.5f, 1.5f, 5f)), arCamera.TransformPoint(new Vector3(0f, 0f, 1.5f)))); 
        })); // 좌측 위 등장 눈덩이 시작)))));
        FirstDialog.Add(new DialogData("/color:black//wait:0.5/안녕, 친구야! 혹시 무슨 고민 있어?", "NoonDung"));
        FirstDialog.Add(new DialogData("/color:black/[(사정을 설명한다.)]", "User", () => ChangeAnimation(noonDungAnimator, "standing"))); // 원하는 애니메이션은 바로 전 대사 뒤에 붙여야 자연스레 이어짐.
        FirstDialog.Add(new DialogData("/color:black/아하, 아직 학교가 처음이라 모르는 게 많다고? 음.. 어디보자~", "NoonDung"));
        FirstDialog.Add(new DialogData("/color:black/그렇지! 숙명여대라면 역시 눈송이! 그 애가 널 도와줄 수 있을 거야!", "NoonDung"));
        FirstDialog.Add(new DialogData("/color:black/같이 학교를 돌아다니면서 /color:blue/눈송이/color:black/가 어디에 있는지 찾아보자!/wait:1//close/", "NoonDung", () => noonDung.SetActive(false)));
        // part2 눈꽃송이 등장
        FirstDialog.Add(new DialogData("/color:black//wait:1/[저기 하늘에 떠 다니는 건 뭐지?]", "User"));
        FirstDialog.Add(new DialogData("/color:black/어디? 어디?", "NoonDung", () => { snowflake.SetActive(true); PlaySound(snowflake);
        //StartCoroutine(MoveObject(snowflake, arCamera.TransformPoint(new Vector3(0.35f, 2f, -1f)), arCamera.TransformPoint(new Vector3(0.35f, 0f, -1f)))); 
        })); // 하늘에서 내려옴 눈꽃송이 시작
        FirstDialog.Add(new DialogData("/color:black//wait:1/우리는! /wait:0.5/학교를 지키는 어벤져스, /click/눈꽃송이들이야!", "Snowflake"));
        FirstDialog.Add(new DialogData("/color:black/마침 잘 만났다! 얘들아, 이 새송이가 눈송이와 친구가 되고 싶대!", "NoonDung"));
        FirstDialog.Add(new DialogData("/color:black/그런거라면... 눈송이에게 줄 선물을 구할 수 있는 이 눈의 결정이 있다면 분명 유용할 거야.", "Snowflake")); //
        FirstDialog.Add(new DialogData("/color:black/마침 우리한테 꿍쳐놓은 눈의 결정이 있으니까, 너한테 줄게!", "Snowflake", () => { ChangeAnimation(snowflakeAnimator1, "standing"); PlaySound(goods);}));
        FirstDialog.Add(new DialogData("/color:black//wait:0.5/눈의 결정 15개를 획득했다!", "Narrator"));
        FirstDialog.Add(new DialogData("/color:black/어려운 친구를 돕는 것도 우리 일이니까. 우리가 새송이를 도와주는 건 어떨까? /click/(뭉치면 산다!)", "Snowflake", () => ChangeAnimation(snowflakeAnimator1, "standing")));
        FirstDialog.Add(new DialogData("/color:black/그래, 눈의 결정이라면 우리가 전문이니까, 함께 다니면서 눈의 결정 찾는걸 도와줄게! /click/(맡겨 줘!)", "Snowflake"));
        FirstDialog.Add(new DialogData("/color:black/[고마워, 눈꽃송이들!]/wait:1//close/", "User", () => { snowflake.SetActive(false); StartCoroutine(ShowPanelFirst());}));
        // part3 로로 등장
        FirstDialog.Add(new DialogData("/color:black//wait:0.5/앗! 찾았다", "RoRo", () => { roro.SetActive(true); PlaySound(roro);
        //StartCoroutine(MoveObject(roro, arCamera.TransformPoint(new Vector3(0f, 1f, 5f)), arCamera.TransformPoint(new Vector3(0f, -0.2f, 1f)))); 
        })); // 멀리서 달려오듯이 등장 로로 시작
        FirstDialog.Add(new DialogData("/color:black/[앗!]", "User", () => ChangeAnimation(roroAnimator, "standing")));
        FirstDialog.Add(new DialogData("/color:black/네가 눈송이를 찾아 다닌다는 새송이 맞지! 소식을 듣고 한달음에 달려왔어!", "RoRo", () => ChangeAnimation(noonDungAnimator, "standing")));
        FirstDialog.Add(new DialogData("/color:black/로로잖아! 과연 학교의 소식통이라 그런지, 소식이 빠르네!", "NoonDung", () => ChangeAnimation(roroAnimator, "standing")));
        FirstDialog.Add(new DialogData("/color:black//emote:Happy/엣헴, 이런 일에 내가 빠질 수 없지. 눈송이와 친구가 되고 싶은 거 맞지?", "RoRo"));
        FirstDialog.Add(new DialogData("/color:black//emote:Happy/눈송이랑 친해지려면 인사를 하고 대화하는 게 중요한데…", "RoRo"));
        FirstDialog.Add(new DialogData("/color:black//emote:Happy/아, 맞다! 1캠퍼스 정문에서 눈송이를 본 것 같다는 걸 알려주려고 온 건데 깜빡했네!", "RoRo"));
        FirstDialog.Add(new DialogData("/color:black//emote:Happy/이러지 말고 직접 가 보는 게 좋겠어!", "RoRo"));
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
        SecondDialog.Add(new DialogData("/color:black//emote:Hello/저기,, 안녕하세요! 처음 보는 분이네요..!", "NoonGyeol", () => { noonkyeol.SetActive(true); PlaySound(noonkyeol);
        //StartCoroutine(MoveObject(noonkyeol, arCamera.TransformPoint(new Vector3(2f, -0.5f, 5f)), arCamera.TransformPoint(new Vector3(0f, -0.5f, 3f)))); 
        })); // 오른쪽에서 천천히 등장 눈결이 시작));
        SecondDialog.Add(new DialogData("/color:black/눈결이 안녕! 혹시 근처에서 눈송이 못봤어?", "NoonDung", () => ChangeAnimation(noonkyeolAnimator, "standing")));
        SecondDialog.Add(new DialogData("/color:black/눈송이 말인가요? 음... 못 봤어요. 무슨 일이신데요?", "NoonGyeol"));
        SecondDialog.Add(new DialogData("/color:black/[(눈결이에게 사정을 설명한다)]", "User"));
        SecondDialog.Add(new DialogData("/color:black//emote:Look/앗, 그렇다면 이 지도가 도움이 될 거예요!", "NoonGyeol", () => { ChangeAnimation(noonkyeolAnimator, "standing"); PlaySound(map); StartCoroutine(ShowPanelSecond());}));
        //지도 기능 해금
        SecondDialog.Add(new DialogData("/color:black//emote:Look/지도는 화면 하단의 버튼을 누르면 볼 수 있어요! 한번 보시겠어요?", "NoonGyeol", () =>{ StartCoroutine(WaitForMapToOpen()); }));
        //유저가 지도를 누르면, 잠시 대기 후 눈결이 대사 스크립트가 지도 위에 뜬다.//
        SecondDialog.Add(new DialogData("/color:black//wait:0.5/지금은 지도가 전부 잠겨 있죠?", "NoonGyeol"));
        SecondDialog.Add(new DialogData("/color:black/교내를 돌아다니면서 많은 눈송이들과 친구가 되면 지도를 해금할 수 있을 거예요!", "NoonGyeol"));
        SecondDialog.Add(new DialogData("/color:black/위쪽의 <을 누르면 지도가 사라져요.", "NoonGyeol"));
        //유저가 지도를 다시 누르면, 지도가 사라진다.
        
        SecondDialog.Add(new DialogData("/color:black//emote:Study/그 외에도 저는 이것저것 많은 것을 알고 있으니까, 제 지식이 도움이 될 수 있을 것 같아요.", "NoonGyeol"));
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
        ThirdDialog.Add(new DialogData("/color:black/안녕, 친구들? /click//emote:Excite/처음 보는 친구도 있구나! 새송이인가 보네?", "KkotSong", () => { kkotsong.SetActive(true); PlaySound(kkotsong);
            //StartCoroutine(MoveObject(kkotsong, arCamera.TransformPoint(new Vector3(0f, -0.3f, 2f)), arCamera.TransformPoint(new Vector3(0f, -0.3f, 2f)))); 
        })); // 화면 가운데서 춤 연습중 꽃송이 시작
        ThirdDialog.Add(new DialogData("/color:black//emote:Hello/이 친구는 꽃송이야! 눈송이의 베프인 꽃송이라면 눈송이가 어디 있는지 알 지도 몰라!", "RoRo", () => ChangeAnimation(kkotsongAnimator, "standing")));
        ThirdDialog.Add(new DialogData("/color:black/눈송이? 너희 눈송이를 찾고 있니?", "KkotSong"));
        ThirdDialog.Add(new DialogData("/color:black/맞아요. 새송이가 눈송이와 친구가 되고 싶대요.", "RoRo"));
        ThirdDialog.Add(new DialogData("/color:black//emote:Happy/그렇다면 정확히 찾아 왔어. 마침 방금 전까지 눈송이랑 함께 있던 참이었거든.", "KkotSong", () => ChangeAnimation(kkotsongAnimator, "standing")));
        ThirdDialog.Add(new DialogData("/color:black/아마 눈송이는 /color:blue/프라임관/color:black/에 있을 거야!", "KkotSong"));
        ThirdDialog.Add(new DialogData("/color:black/참, 눈송이한테는 이걸 주면 좋아할 거야. 가서 말을 건 뒤 선물을 줘 봐!", "KkotSong", () => { kkotsong.SetActive(false); dialogTriggered[2] = true; uiController.onClickInventoryButton(); StartCoroutine(GetEmblemBadge());}));
        //엠블럼 뱃지 획득
        ThirdDialog.Add(new DialogData("/color:black/엠블럼 뱃지를 획득했다!", "Narrator"));
        ThirdDialog.Add(new DialogData("/color:black/왼쪽 두번째 인벤토리 아이콘을 클릭해보세요", "Narrator"));

        DialogManager.Show(ThirdDialog);
    }

    // 6~7
    public void FourthDialog()
    {
        if (dialogTriggered[3]) return;
        if (!dialogTriggered[2]) return; // 이전 다이얼로그가 호출되지 않았으면 return

        dialogTriggered[3] = true;

        var FourthDialog = new List<DialogData>();

        //part 6,7

        // 눈송이 등장
        FourthDialog.Add(new DialogData("/color:black//emote:Happy/찾았다, 눈송이!", "KkotSong", () => { noonsong.SetActive(true); PlaySound(noonsong);
        //StartCoroutine(MoveObject(noonsong, arCamera.TransformPoint(new Vector3(-1.5f, -0.3f, 5f)), arCamera.TransformPoint(new Vector3(0f, -0.3f, 2f)))); 
        })); // 화면 좌측에서 등장 눈송이 시작
        FourthDialog.Add(new DialogData("/color:black//emote:Happy/어서 가서 인사해봐!", "RoRo", () => StartCoroutine(GreetNooonsong())));
        //플레이어가 인사하기를 눌러 눈송이에게 인사한다.
        FourthDialog.Add(new DialogData("/color:black//wait:0.5//emote:Excite/안녕, 친구들! 어라, 처음 보는 친구도 있네?", "NoonSong"));
        FourthDialog.Add(new DialogData("/color:black/이 애가 너와 친구가 되고 싶다고 해서 데려왔어!", "NoonDung"));
        FourthDialog.Add(new DialogData("/color:black/눈송이를 위한 선물도 가져왔어! (두근두근…!)", "Snowflake", () => { ChangeAnimation(noonsongAnimator, "standing"); StartCoroutine(GiveGiftNooonsong());}));
        //플레이어가 선물하기를 눌러, 눈송이에게 숙명여대 앰브럼 뱃지 아이템을 선물한다
        FourthDialog.Add(new DialogData("/color:black//wait:0.5//emote:Love/와아, 숙명여대 앰블럼 뱃지네! 정말 기뻐!", "NoonSong"));
        //눈송이 호감도 오르는 연출
        FourthDialog.Add(new DialogData("/color:black/이렇게 찾아와 줘서 고마워, 그럼 우리 오늘부터 친구 하자!", "NoonSong"));
        FourthDialog.Add(new DialogData("/color:black/[(눈송이와 친구가 되자.)]/wait:2.0/", "User", () =>  noonsong.SetActive(false)));
        //시스템 상으로 눈송이와 친구가 되고 눈송이가 도감에 추가됨
        FourthDialog.Add(new DialogData("/color:black/아래 도감 보여? 도감 버튼을 누르면 여태까지 만나고 친구가 된 눈송이들을 볼 수 있어.", "NoonDung", () => { noonDung.SetActive(true); PlaySound(noonDung); StartCoroutine(ActivateBookCanvas());}));
        FourthDialog.Add(new DialogData("/color:black//wait:0.5/물론 우리 눈송 프렌즈들의 정보도 있지!", "NoonDung", () => { StartCoroutine(DiscoverAllEntries()); }));
        //유저 도감 버튼을 누르면 도감으로 이동, 잠시 대기 후에 눈덩이 대화창이 도감 위에 나타남
        FourthDialog.Add(new DialogData("/color:black/아이콘을 누르면 친구의 정보를 볼 수 있어~ 눈송이를 눌러 보자!", "NoonDung", () => StartCoroutine(clickNoonsong())));
        //유저가 눈송이 아이콘을 누르면 눈송이 설명이 뜸.
        FourthDialog.Add(new DialogData("/color:black/눈송이의 이름과 설명이 보이지? 특별한 눈송이들을 만나면 호감도도 확인할 수 있을 거야.", "NoonDung"));
        FourthDialog.Add(new DialogData("/color:black/참, 화면 위쪽의 카메라 모양을 누르면 원하는 친구와 사진을 찍을 수도 있어!", "NoonDung"));
        FourthDialog.Add(new DialogData("/color:black/위쪽의 X를 누르면 원래 화면으로 돌아갈 수 있어!", "NoonDung", () => { noonDung.SetActive(false); StartCoroutine(clickXButton());}));
        //유저가 도감 종료 입력을 하면 원래 화면으로 돌아감.

        // 튜리 등장
        FourthDialog.Add(new DialogData("/color:black/앗! 다들 나만 빼고 여기 모여 있었구나!", "Turi", () => { turi.SetActive(true); PlaySound(turi);
        //StartCoroutine(MoveObject(turi, arCamera.TransformPoint(new Vector3(0f, -2f, 3f)), arCamera.TransformPoint(new Vector3(0f, 0f, 3f)))); 
        })); // 아래에서 등장 튜리 시작
        FourthDialog.Add(new DialogData("/color:black/어라? 못 보던 얼굴도 있네?", "Turi"));
        FourthDialog.Add(new DialogData("/color:black/[(인사한다.)]", "Turi", () => ChangeAnimation(turiAnimator, "standing")));
        FourthDialog.Add(new DialogData("/color:black/안녕, 튜리! 이 애는 새로운 눈송이인데, 나랑 친구가 되고 싶다고 찾아와 줬어!", "NoonSong"));
        FourthDialog.Add(new DialogData("/color:black/오… 이해했어!", "Turi"));
        FourthDialog.Add(new DialogData("/color:black/거기 눈송, 좀 더 다양한 눈송이를 만나보고 싶지 않아?", "Turi"));
        FourthDialog.Add(new DialogData("/color:black/[(고개를 끄덕인다.)]", "User", () => ChangeAnimation(turiAnimator, "standing")));
        FourthDialog.Add(new DialogData("/color:black/흐흥, 미래 산업시대의 선구자가 될 이 튜리님에게 너 같은 친구들을 위한 발명품이 있지!", "Turi"));
        FourthDialog.Add(new DialogData("/color:black/바로바로… 특별한 눈송이들의 마음도 사로잡을 수 있는 선물 아이템들이야!", "Turi", () => ChangeAnimation(turiAnimator, "standing")));
        FourthDialog.Add(new DialogData("/color:black/어때? 가지고 싶지? 그 대신, 선물 아이템들을 만드는 데에는 눈의 결정이 많이 필요해.", "Turi"));
        FourthDialog.Add(new DialogData("/color:blue/제2캠퍼스 눈꽃 광장/color:black/에 있는 내 연구실에 오면 다양한 선물 아이템들로 바꿔줄게! 잊지 말고 놀러 와~!", "Turi", () => { turi.SetActive(false); }));
        FourthDialog.Add(new DialogData("/color:black/튜토리얼을 성공적으로 마친 당신에게 이것을 드립니다!", "Narrator", () => { PlaySound(profile); StartCoroutine(ShowPanelSeventh());}));
        FourthDialog.Add(new DialogData("/color:black//wait:0.5/다양한 눈송이들과 친구가 되어 도감을 전부 채우는 것을 목표로 힘내요!!", "Narrator", () => { CompleteTutorial();}));
       
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
            { turi, turiSound },
            { map, getMapSound },
            { goods, getGoodsSound },
            { profile, getProfileSound }

        };

        //FirstDialog();
        // dialogTriggered[0] = true;
        // SecondDialog();
        // dialogTriggered[1] = true;
        // ThirdDialog();
        // dialogTriggered[2] = true;
        // FourthDialog();

        switch(UserDataManager.Instance.getSave())
        {
            case 0:
                FirstDialog();
                break;
            case 1:
                Debug.Log("2");
                SecondDialog();
                break;
            case 2:
                Debug.Log("3");
                ThirdDialog();
                break;
            case 3:
                Debug.Log("4");
                FourthDialog();
                break;
            case 4:
                CompleteTutorial();
                break;
            default:
                Debug.Log(UserDataManager.Instance.getSave());
                break;
        }
        
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
    // private IEnumerator MoveObject(GameObject obj, Vector3 startPos, Vector3 endPos)
    // {
    //     // 초기 위치 설정
    //     obj.transform.position = startPos;

    //     // 오브젝트에 할당된 사운드가 있는지 확인하고 재생
    //     if (objectSoundMap.TryGetValue(obj, out AudioClip assignedSound))
    //     {
    //         AudioSource.PlayClipAtPoint(assignedSound, obj.transform.position);
    //     }

    //     // 이동 애니메이션 실행
    //     float elapsedTime = 0f;
    //     while (elapsedTime < moveDuration)
    //     {
    //         elapsedTime += Time.deltaTime;
    //         float t = Mathf.Clamp01(elapsedTime / moveDuration);
    //         obj.transform.position = Vector3.Lerp(startPos, endPos, t);
    //         yield return null;
    //     }

    //     // 정확한 최종 위치로 설정
    //     obj.transform.position = endPos;
    // }

    private void PlaySound(GameObject obj)
    {
        if (objectSoundMap.TryGetValue(obj, out AudioClip assignedSound))
        {
            AudioSource audioSource = obj.GetComponent<AudioSource>();
            if (audioSource == null)
            {
                audioSource = obj.AddComponent<AudioSource>();
            }
            audioSource.clip = assignedSound;
            audioSource.Play();
        }
    }

    // private IEnumerator ShowMainTuto()
    // {
    //      Time.timeScale = 0f;
    //     mainCanvas.SetActive(true);

    //     // 첫 번째 클릭 대기
    //     while (!Input.GetMouseButtonDown(0))
    //     {
    //         yield return null;
    //     }
    //     yield return new WaitForSecondsRealtime(0.1f);

    //     // mainPanel 활성화
    //     mainPanel.SetActive(true);

    //     // 두 번째 클릭 대기
    //     while (!Input.GetMouseButtonDown(0))
    //     {
    //         yield return null;
    //     }

    //     // mainPanel & mainCanvas 비활성화
    //     mainPanel.SetActive(false);
    //     mainCanvas.SetActive(false);
    //     Time.timeScale = 1f;

    // }

    
    //재화팝업
    private IEnumerator ShowPanelFirst()
    {
        Time.timeScale = 0f; // 시간 정지
                             // 패널 활성화
        uiController.PopUpGoodsPanel();

        while (!uiController.IsButtonClicked()) // 마우스 클릭을 기다림
        {
            yield return null; // 한 프레임을 대기
        }
        Count.SetActive(true);
        Time.timeScale = 1f; // 시간 재개
    }

    //지도팝업
    private IEnumerator ShowPanelSecond()
    {
        Time.timeScale = 0f; // 시간 정지

        uiController.PopUpMapPanel();

        while (!uiController.IsButtonClicked()) // 마우스 클릭을 기다림
        {
            yield return null; // 한 프레임을 대기
        }
        Time.timeScale = 1f; // 시간 재개
    }

    private IEnumerator WaitForMapToOpen()
    {
        Time.timeScale = 0f; // 시간 정지
        uiController.onClickMapButton();

        while (!mapCanvas.gameObject.activeSelf)
        {
            printerText.text = "오른쪽 첫번째 지도 아이콘을 클릭해보세요";
            yield return null; // mapCanvas가 활성화될 때까지 대기

        }
        yield return new WaitForSecondsRealtime(2f);
        printerText.text = "";

        mapCanvas.sortingOrder = 0;
        Time.timeScale = 1f; // 시간 재개
    }


    private IEnumerator GreetNooonsong()
    {
        Time.timeScale = 0f; // 시간 정지
        uiController.onClickInteractoinButton();

        while(!uiController.IsButtonClicked())//인사버튼 클릭 대기
        {
            printerText.text = "하단의 말풍선 아이콘을 클릭해 눈송이와 인사해보세요";
            yield return null; // 한 프레임을 대기
        }
        printerText.text = "";

        Time.timeScale = 1f; // 시간 재개
    }

    private IEnumerator GiveGiftNooonsong()
    {
        Time.timeScale = 0f; // 시간 정지
        uiController.onClickGiftButton();

        while(!giftCanvas.gameObject.activeSelf)//선물캔버스가 활성화될때까지 대기
        {
            printerText.text = "선물 아이콘을 클릭해 눈송이에게 아이템을 선물해보세요";
            yield return null; // 한 프레임을 대기
        }
        printerText.text = "";


        bool isPopupOpened = false;
        slotButton.onClick.AddListener(() =>
        {
            isPopupOpened = true;
            popupUI.SetActive(true); // 팝업 UI 활성화
        });

        while (!isPopupOpened)
        {
            yield return null; // 한 프레임 대기
        }

        // 팝업 UI 처리
        bool isGiftGiven = false;

        // 선물하기 버튼 클릭 시 처리
        giftButton.onClick.AddListener(() =>
        {
            isGiftGiven = true;
            Destroy(slotButton.gameObject);
            giftCanvas.gameObject.SetActive(false);
            popupUI.SetActive(false); // 팝업 UI 비활성화
            StartCoroutine(ActivateEffectCoroutine());
        });

        // 취소하기 버튼 클릭 시 처리
        cancelButton.onClick.AddListener(() =>
        {
            popupUI.SetActive(false); // 팝업 UI 비활성화
        });

        // 선물하기 버튼 클릭될 때까지 대기
        while (!isGiftGiven)
        {
            yield return null; // 한 프레임 대기
        }

        Time.timeScale = 1f; // 시간 재개
    }


    //눈송 프랜즈만 발견 처리
    private IEnumerator DiscoverAllEntries()
    {
        Time.timeScale = 0f; // 시간 정지

        FriendsManager friendsManager = FindObjectOfType<FriendsManager>();

        while (friendsManager == null) // FriendsManager 가 씬에서 발견될 때까지 대기
        {
            yield return null; // 한 프레임 대기
            friendsManager = FindObjectOfType<FriendsManager>();
        }

        friendsManager.SetAllEntriesDiscovered();

        Time.timeScale = 1f; // 시간 재개
    }

    //앰블럼 뱃지만 획득 처리
    private IEnumerator GetEmblemBadge()
    {
        Time.timeScale = 0f; // 시간 정지

        InventoryManager inventoryManager = FindObjectOfType<InventoryManager>();
        while (inventoryManager == null) // InventoryManager가 씬에서 발견될 때까지 대기
        {
            yield return null; // 한 프레임 대기
            Debug.Log("인벤토리 매니저 찾는 중");
            inventoryManager = FindObjectOfType<InventoryManager>();
        }

        inventoryManager.AddEmblemBadge();

        Time.timeScale = 1f; // 시간 재개
    }

    private IEnumerator ActivateEffectCoroutine()
    {
        effectObject.SetActive(true);
        yield return new WaitForSeconds(5f);
        effectObject.SetActive(false);
    }


    private IEnumerator ActivateBookCanvas()
    {
        Time.timeScale = 0f; // 시간 정지
        uiController.onClickBookButton();

        while(!bookCanvas.gameObject.activeSelf)
        {
            printerText.text = "오른쪽 세번째 도감 아이콘을 클릭해보세요";
            yield return null; // 한 프레임을 대
        }
        printerText.text = "";
        StartCoroutine(ShowPanelThird());
        Time.timeScale = 1f; // 시간 재개
    }

    private IEnumerator ShowPanelThird()
    {
        Time.timeScale = 0f; // 시간 정지
        uiController.PopUPBookPanel();

        while ((!uiController.IsButtonClicked())) // 마우스 클릭을 기다림
        {
            yield return null; // 한 프레임을 대기
        }
        while (uiController.IsPanelActive()) // 패널이 활성화된 상태라면 기다림
        {
            yield return null;
        }

        StartCoroutine(ShowPanelFourth());
        Time.timeScale = 1f; // 시간 재개
    }

    private IEnumerator ShowPanelFourth()
    {
        Time.timeScale = 0f; // 시간 정지

        uiController.PopUpLikePanel();

        while (!uiController.IsButtonClicked()) // 마우스 클릭을 기다림
        {
            yield return null; // 한 프레임을 대기
        }
        bookCanvas.sortingOrder = 0;
        Time.timeScale = 1f; // 시간 재개
    }

    private IEnumerator clickNoonsong()
    {
        Time.timeScale = 0f; // 시간 정지
        bookCanvas.sortingOrder = 2;

        yield return new WaitForSecondsRealtime(5f);

        bookCanvas.sortingOrder = 0;
        Time.timeScale = 1f; // 시간 재개
    }

    private IEnumerator clickXButton()
    {
        Time.timeScale = 0f; // 시간 정지
        bookCanvas.sortingOrder = 2;
        while((!uiController.IsButtonClicked()))
        {
            yield return null; // 한 프레임을 대기
        }
        yield return new WaitForSecondsRealtime(1.5f);
        Time.timeScale = 1f; // 시간 재개

    }

    private IEnumerator ShowPanelSeventh()
    {
        Time.timeScale = 0f; // 시간 정지
        StudentId.SetActive(true);
        while (!Input.GetMouseButtonDown(0)) // 마우스 클릭을 기다림
        {
            yield return null; // 한 프레임을 대기
        }

        Time.timeScale = 1f; // 시간 재개
    }

    public void CompleteTutorial()
    {
        BackendSavePoint.Instance.SaveGameData(4);

        Debug.Log("튜토리얼 완료");
        SceneManager.LoadScene("MainScene(Release)");
    }

}