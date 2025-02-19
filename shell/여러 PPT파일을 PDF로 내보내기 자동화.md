> ### 📄 1. 문제 개요

* 다음 ppt들은 인프런의 **[Backend 멀티쓰레드 이해하고 통찰력 키우기](https://inf.run/JAtvq)** 강좌에 첨부된 파일들 이지만,
* 이걸 pdf로 보고 싶었다, 그런데 14장이나 되는 ppt를  일일이 pdf로 열었다 닫았다 하는건 너무~~~ 노가다임

<div align=center>
    <img src="https://imgur.com/aHjreBD.png">
    <h5></h5>
</div>

> **이걸 CLI를 통해 Shell 코드로 자동화 할 수 없을까?** 해서.. 찾아 보게 되었다.

* 본인의 컴퓨터는 mac이고, "MS Office Powerpoint"의 cli를 찾아보기로 했지만, 
딱히 만족스러운 결과가 나오지는 않았다..

---

> ### 📄 2. LibreOffice

* 검색을 하다보니 **"LibreOffice"** 라는 프로그램을 발견했다. 
오픈소스이고 Linux, Max, MicroSoft 모든 플랫폼을 지원한다.

* 무엇 보다도 CLI를 제공해 쉘 스크립팅으로 자동화 하기 딱 알맞았다.

---

#### 1). `.dmg` 설치

<div align=center>
    <img src="https://imgur.com/zMQT3b3.jpeg">
    <h5><a href="https://www.libreoffice.org/download/download-libreoffice/">공식 : LibreOffice 사이트</a></h5>
</div>

* 여기서 `.dmg` 파일을 받아 주도록 하고 설치를 하자.

---

#### 2). `~/.zshrc` iTerm 환경변수 설정 

* LibreOffice로 파일 매니징을 제공하는 CLI 커맨드는 `soffice`이다.

<div align=center>
    <img src="https://imgur.com/fHwE3AG.jpeg">
    <h5>그 커맨드는 다운받은 "LibreOffice.app > 패키지 내용 보기> Contents > MacOS" 안에 들어있다.</h5>
</div>

* 그리고 나서 `~/.zshrc`을 열고 다음 텍스트를 아무데나 넣어주자
    ```shell
    # LibreOffice cli
    export PATH="/Applications/LibreOffice.app/Contents/MacOS:$PATH"
    ```

* 주의할 점은 “~/Applications”이 아니라 더 상위 디렉토리인 “/Applications/…” 이란 것이다.. 
루트 전치사 실수로 한번 잘못 붙여서 계속 CLI 앱을 실행할 수 없었다.. **주의하자!**

---

#### 3). `soffice` 가 동작하는지 테스트 해보자.

* `soffice --help` 입력시 아래와 같이 사용법에 대해 나온다면 환경 변수 설정이 잘 된것이다.
    ```bash
    soffice --help
    LibreOffice 24.8.4.2 bb3cfa12c7b1bf994ecc5649a80400d06cd71002

    Usage: soffice [argument...]
           argument - switches, switch parameters and document URIs (filenames).

    Using without special arguments:
    ...
    ```

* 그렇다면 실제로 .pptx가 ppt로 되는지 테스트로 실행해보자
    ```bash
    soffice --headless --convert-to pdf "11_CPU Cache Line.pptx"
    convert ~/Develop Projects/CsharpProject/UdemyMultiThreading/MultiThreading_Test/doc/ppts/11_CPU Cache 
    Line.pptx as a Impress document -> ~/Develop Projects/CsharpProject/UdemyMultiThreading/
    MultiThreading_Test/doc/ppts/11_CPU Cache Line.pdf using filter : impress_pdf_Export
    ```

<div align=center>
    <img src="https://imgur.com/Lw1HLsn.png">
    <h5>오.. 실제로 작동 했잖아?</h5>
</div>

---

#### 4). 자동화 쉘 스크립트를 작성해 보자.

* 이제 `ls -1`로 디렉토리의 파일들을 개행 문자로 잘라서 받아, 파일의 이름을 공백을 허용하도록 하자.
    ```shell
    #!/bin/bash
    # sh pptx_to_pdf_with_libreoffice.sh <inputs에 들어갈 ppts경로>
    IFS=$'\n'

    inputs="$1"
    currentDir=$(pwd)
    cd $inputs

    files=($(ls -1))
    # echo $files

    for file in "${files[@]}"; do
        # echo "파일: $file"
        soffice --headless --convert-to pdf $file
    done

    cd $currentDir
    ```

* **결과는 성공적**
<div align=center>
    <img src="https://imgur.com/GfRU0g2.png">
    <h5>ppts에서 pdf로 잘 변환된 모습이다!</h5>
</div>

