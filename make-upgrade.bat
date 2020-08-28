@rem
@rem Script para atualização das versões de dependências declaradas no pack.info.
@rem
@rem O algoritmo checa o repositório do PackDm e modifica o pack.info no
@rem processo para que as dependências reflitam as versões mais recentes.
@rem
@rem O script falha se houverem recursos não-commitados.
@rem

if not exist toolset.exe echo | call make-install-deps.bat
@if errorlevel 1 (
  @echo.
  @echo.[ERR]Nao foi possivel baixar a ferramenta: toolset.exe
  @pause
  @exit /b %errorlevel%
)

rem Atualizando a versão das dependências
toolset upgrade
@if errorlevel 1 (
  @echo.
  @echo.[ERR]Nao foi possivel atualizar a versao das dependências.
  @pause
  @exit /b %errorlevel%
)

@echo.
@echo.[OK]Feito!
@pause
