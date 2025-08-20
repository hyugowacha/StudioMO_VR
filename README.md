# Unimo : Beyond 
## Unity VR 기업협약 프로젝트
<img width="313" height="174" alt="image" src="https://github.com/user-attachments/assets/4f6178f2-5c79-4f67-a8cc-2eea2cc81501" />
<img width="362" height="201" alt="image" src="https://github.com/user-attachments/assets/8ea85ed4-04bc-4a82-876e-34c2961345d2" />

------
### 프로젝트 소개📌

장르: VR 닷지 슈팅

개발 기간: 2025.05.08 ~ 2025.07.02 (56일)

개발 인원: 8명 (기획 4명 + 개발 4명)

담당 파트: 탄막 시스템, 옵션UI, 맵 비주얼 이펙트, etc

------

### 기술 상세💡

+ ### 탄막 시스템
  <img width="963" height="469" alt="image" src="https://github.com/user-attachments/assets/31386768-d428-4b0e-99f4-c6309b5fc8d5" />

  음악 BPM과 CSV 패턴 데이터를 기반으로 탄막을 생성하는 시스템

  ### 핵심 기능

   + ### BulletPatternLoader.cs

    +  패턴형 탄막 CSV파일을 파싱하여 데이터 리스트에 저장 후 CSV 파일에서 탄막 패턴 데이터를 읽어와 다른 곳에서 사용될 수 있도록 List로 파싱함
    

    <img width="753" height="194" alt="image" src="https://github.com/user-attachments/assets/feafbc43-c0bd-4812-9cdb-69901957a11c" />

    <img width="573" height="87" alt="image" src="https://github.com/user-attachments/assets/244f2663-9a1a-4e09-9aa7-235119413b8d" />
    
    
      CSV의 첫번째 줄(헤더)을 기반으로 열의 개수를 파악 후
    
      beat_timing(발사 시점), Bullet_preset_ID(판막 패턴 유형), generate_preset(생성 위치), Bullet_amount(총알 발사량), fire_angle(발사각), Bullet_angle(각도형 패턴), Bullet_range(거리형 패턴)

      순서로 값을 읽어와 BulletSpawnData 형태로 만들어 패턴 데이터 리스트에 저장함

   + ### BulletPatternExecutor.cs
     
    <img width="290" height="158" alt="image" src="https://github.com/user-attachments/assets/5687319d-9ca7-4e7a-b5c6-cc4e3bfa9e3b" />

   
     +  CSV 데이터에 맞춰 탄막 패턴을 실행하는 컨트롤러

     BulletPatternLoader로부터 CSV 데이터를 로드받은 후 비트형 탄막과 패턴형 탄막을 분리하여 각자 실행함
  
     
    - #### void ProcessPatternTiming
     
     -> 게임 시작 시간을 기반으로 그 시간이 되었을 때 탄막 실행

    - #### void ExecutePattern
     
      bulletPresetID(탄막 패턴의 배치를 결정하는 방식을 구분하기 위한 ID)에 따라 각자 다른 탄막 스폰

       -ID = 1 -> SpawnPatternAngle(각도형 패턴 탄막 스폰) 메서드 실행

       -ID = 2 -> SpawnPatternRange(거리형 패턴 탄막 스폰) 메서드 실행

   + ### (Angle/Range)PatternSpawner.cs
     
     <img width="308" height="296" alt="image" src="https://github.com/user-attachments/assets/d633a620-947d-4c8b-a364-ec9917d1757c" />

     벽(사이드) 위치와 발사 프리셋(preset ID)에 따라 탄막 발사 위치를 계산하고, 지정 각도로 패턴형 탄막을 생성함

    + #### void CalculateSpawnPosition

      각 사이드(1~4)에 대해 9개의 프리셋 위치를 계산함

      Mathf.Lerp(min, max, t)를 통해 벽의 양 끝(min~max) 사이를 균등 간격으로 나눈 후 spawnPositions딕셔너리에 사이드별 스폰 위치를 저장함

    + #### void FireAnglePatternBullet

      특정 프리셋 좌표에서 지정 각도·간격으로 탄막 발사
    
      spawnPositions에서 발사 위치를 가져온 후 사이드 방향에 따라 기본 벡터를 설정함

--------

+ ### 옵션 UI
  
<img width="799" height="373" alt="image" src="https://github.com/user-attachments/assets/9631df61-9732-4616-bdcb-435b0ac2599e" />

VR 플레이어의 커스터마이즈 및 환경 설정 기능

### 핵심 기능

   + ### PlayerOptionUI.cs

      플레이어의 스킨 변경 관련, 회전 방법 변경 메서드를 포함하는 클래스

     + #### void SelectSkin
    
        플레이어의 프로필 사진 변경사항을 적용시키는 메서드. ScriptableObject로 스킨 데이터를 관리

     + #### void ChangeTurnMethod
    
        ‘스냅’ 또는 ‘부드러운 회전’(스무스) 중 선택. Input System 설정을 기반으로 처리

   + ### AudioOptionUI.cs

     게임의 오디오 관련 요소를 조정할 수 있는 기능을 포함한 클래스
     
     + #### void BGMController / SFXController
     
       AudioMixer와 연동하여 게임의 배경음악/효과음을 조정하게 하는 메서드

       슬라이더를 통해 볼륨 값을 받아올 수 있음
  
   + ### LanguageOptionUI.cs

     플레이어의 사용 언어를 변경할 수 있는 기능을 포함한 클래스

     + #### void BGMController / SFXController

     <img width="709" height="172" alt="image" src="https://github.com/user-attachments/assets/4c9c51ce-9df0-4be9-b986-a15cd1ca98e6" />

     플레이어 관련 정보는 PlayerPrefs로 저장되며, Save 버튼을 누를 시 변경 사항이 최종적으로 저장됨

+ ### 맵 비주얼 이펙트
    
  <img width="406" height="285" alt="image" src="https://github.com/user-attachments/assets/a6d2fb11-2165-4967-8144-d336a41f135b" />
  <img width="406" height="285" alt="image" src="https://github.com/user-attachments/assets/e803037c-6176-42a6-8591-3d11274e0e5a" />
  
게임 맵의 비주얼적 요소를 더하기 위해 외곽에 이펙트를 스폰시키는 역할을 함
  
### 핵심 기능
 
   + ### MapEffectSpawner.cs

     맵 종류(Sea / Glacier / Desert / Lava / Labyrinth)에 따른 이펙트 스폰

------------------

### 기술 스택 🛠
 
+ #### Engine: Unity (C#)

+ #### VR : Unity XR Interaction Toolkit

+ #### 데이터 관리 : CSV 기반 탄막 패턴 로딩

+ #### UI/UX : Unity UI, ScriptableObject 기반 스킨 시스템

     

     


     

      
   
      

    


    
