@For /f "tokens=1* delims=" %%A in (
	'reg query HKCR /f "URL:*" /s /d ^| findstr /i /c:"URL:" ^| findstr /v /c:"URL: " ^| sort'
) Do @Echo %%A %%B

echo Done. Press any key to exit. 
pause >nul