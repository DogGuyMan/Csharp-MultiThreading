# Udemy 멀티스레드 강좌 공부 노트

#### 다음 강좌를 C#으로 제작
> #### [Udemy : Java 멀티스레딩, 병행성 및 성능 최적화 - 전문가 되기](https://www.udemy.com/course/java-multi-threading/?couponCode=KEEPLEARNING)

---

> ### 1. [Vscode 솔루션 파일 제작](https://stackoverflow.com/questions/36343223/create-c-sharp-sln-file-with-visual-studio-code)

```shell
dotnet new console
```

---

> ### 2. vscode IntelliCode

* 솔루션 파일만 제작했다 하더라도 인텔리 센스가 작동 안할 때가 있다.
* 그럴떄는 폴더 구조가 루트 디렉토리에 `.vscode` 폴더 만들고, setting.json 파일을 만들어야 한다.
  ```
    ROOT
      ├── .vscode
      │   └── settings.json
      └── UdemyMultiThreading
          └── MultiThreading_Test
            ├── MultiThreading_Test.csproj
            ├── MultiThreading_Test.sln
            ├── README.md
            ├── doc
            ├── img
            ├── resources
            └── scripts
              ├── Lecture1_Program.cs
              ├── Lecture2_FactorialProgram.cs
              ├── Lecture3_HTTPProgram.cs
              ├── Lecture3_ImageProgram.cs
              └── main.cs
  ```
* Preferences: Open Workspace Settings (JSON)
  ![](image/2024-11-17-20-17-14.png)
  ```json
  /* setting.json */
  {
    "dotnet.defaultSolution": "TestConsole.sln"
  }
  ```

---

> ### 3. NuGet 패키지 관리

![](./image/2024-03-16-02-26-34.png)

1. ##### 터미널로 패키지관리
   * VS Code에서 내장 터미널을 열고(Ctrl+`` 또는 Cmd+`` on macOS), 프로젝트 디렉토리로 이동
   * dotnet add package [패키지 이름] 명령어를 사용하여 NuGet 패키지를 프로젝트에 추가.
     * *예를 들어, Newtonsoft.Json 패키지를 추가하려면 dotnet add package Newtonsoft.Json 명령을 사용*
 
2. ##### NuGet 으로 확장 기능 사용
   * VS Code NuGet 패키지를 검색
   * NuGet Package Manager 확장 기능을 설치시 VS Code 내에서 직접 NuGet 패키지를 검색하고 설치할 수 있음
   * 커맨드 팔레트(Ctrl+Shift+P 또는 Cmd+Shift+P on macOS)를 열고 해당 확장 기능의 명령을 사용하여 패키지를 관리할 수 있습니다.

#### 4). 프로젝트 빌드 및 실행

1. ##### 빌드
   * NuGet 패키지를 프로젝트에 추가한 후, 
   * 터미널에서 dotnet build 명령을 실행하여 프로젝트를 빌드

2. ##### 프로젝트 실행
   * dotnet run 명령을 실행하여 프로젝트를 실행할 수 있습니다.
      ```shell
      cd MultiThreading_Test
      dotnet run
      ```
3. ##### 디버깅
     1. csproj 리소스와 결과디렉토리를 명시한다
          ```xml
          <ItemGroup>
            <PackageReference Include="SixLabors.ImageSharp" Version="3.1.5" />
            <!-- 
              여기서 부터 Content하고 include 하고 감싸준다.
            --> 
            <Content Include="resources/*.*">
              <CopyToOutputDirectory>Always</CopyToOutputDirectory>
            </Content>
            <Content Include="out/*.*">
              <CopyToOutputDirectory>Always</CopyToOutputDirectory>
            </Content>
          </ItemGroup>
          ```
     2. launch.json도 수정하자
     3. ![](image/2024-12-10-19-33-49.png)

> ### 4. 추가한 패키지

* #### [SixLabors.ImageSharp](https://docs.sixlabors.com/index.html)
  * System.Drawing.Common은 Apple M1에서 지원하지 않아, SixLabors.ImageSharp을 사용
