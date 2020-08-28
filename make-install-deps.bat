@rem
@rem Script de instalacao de ferramentas e dependencias do projeto.
@rem

rem Baixando a ferramenta de automação
svn export "https://172.27.0.5/svn/fabrica/tools/toolset/Dist/toolset.exe" . --force
@if errorlevel 1 (
  @echo.
  @echo.[ERR]Nao foi possivel baixar a ferramenta de automacao: toolset.exe
  @pause
  @exit /b %errorlevel%
)

rem Baixando as ferramentas usadas pelo toolset
@rem Nao critico. Falhas podem ser ignoradas.
toolset update-toolchain

rem Baixando as dependencias do projeto
toolset restore
@if errorlevel 1 (
  @echo.
  @echo.[ERR]Nao foi possivel baixar as dependencias do PackDm
  @pause
  @exit /b %errorlevel%
)

@echo.
@echo.[OK]Feito!
@pause
