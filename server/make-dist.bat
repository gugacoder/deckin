@rem
@rem Script de compilação e geração da distribuição do projeto.
@rem

if not exist toolset.exe echo | call make-install-deps.bat
@if errorlevel 1 (
  @echo.
  @echo.[ERR]Nao foi possivel baixar a ferramenta: toolset.exe
  @pause
  @exit /b %errorlevel%
)

rem Construindo a solução do VisualStudio
toolset build
@if errorlevel 1 (
  @echo.
  @echo.[ERR]Nao foi possivel compilar a solucao.
  @pause
  @exit /b %errorlevel%
)

@echo.
@echo.[OK]Feito!
@pause
