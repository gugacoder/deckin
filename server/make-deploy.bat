@rem
@rem Script de publicacao dos binarios do projeto no repositorio do PackDm.
@rem

if not exist toolset.exe echo | call make-install-deps.bat
@if errorlevel 1 (
  @echo.
  @echo.[ERR]Nao foi possivel baixar a ferramenta: toolset.exe
  @pause
  @exit /b %errorlevel%
)

if not exist Dist\pack.info echo | call make-dist.bat
@if errorlevel 1 (
  @echo.
  @echo.[ERR]Nao foi possivel compilar o projeto
  @pause
  @exit /b %errorlevel%
)

rem Publicando os binarios
toolset deploy
@if errorlevel 1 (
  @echo.
  @echo.[ERR]Nao foi possivel publicar os binarios no repositorio do PackDm.
  @pause
  @exit /b %errorlevel%
)

@echo.
@echo.[OK]Feito!
@pause
