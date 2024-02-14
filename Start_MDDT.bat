wmic process where caption="javaw.exe" get commandline > temp.txt
>nul find "mddt-client" temp.txt && (echo mddt-client.jar already running) || start "" "%appdata%\Microsoft\Windows\Start Menu\Programs\NaKoMddt\NaKoMddt2 Starten.lnk"
del temp.txt

