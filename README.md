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

  + ### BulletPatternLoader

    패턴형 탄막 CSV파일을 파싱하여 데이터 리스트에 저장하는 스크립트

    + CSV 파일에서 탄막 패턴 데이터를 읽어와 다른 곳에서 사용될 수 있도록 List로 파싱하는 역할을 함

    <img width="753" height="194" alt="image" src="https://github.com/user-attachments/assets/feafbc43-c0bd-4812-9cdb-69901957a11c" />

    <img width="573" height="87" alt="image" src="https://github.com/user-attachments/assets/244f2663-9a1a-4e09-9aa7-235119413b8d" />
    
    CSV의 첫번째 줄(헤더)을 기반으로 열의 개수를 파악 후
    
    beat_timing(발사 시점), Bullet_preset_ID(판막 패턴 유형), generate_preset(생성 위치), Bullet_amount(총알 발사량), fire_angle(발사각), Bullet_angle(각도형 패턴), Bullet_range(거리형 패턴)

    순서로 값을 읽어와 BulletSpawnData 형태로 만들어 패턴 데이터 리스트에 저장함

    


    
