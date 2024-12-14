if [[ $(lsof -i tcp:8000) ]]; then
    echo " Type \"kill -9 PID\" "
else 
    echo " No Process In Port 8000 "
fi