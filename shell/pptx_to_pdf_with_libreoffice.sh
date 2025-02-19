IFS=$'\n'

# sh pptx_to_pdf_with_libreoffice.sh <inputs에 들어갈 ppts경로>
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