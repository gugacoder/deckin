-- drop table mlogic.TBsis_config;
if object_id('mlogic.TBsis_config') is null
begin
  create table mlogic.TBsis_config (
    DFchave varchar(400) not null
      constraint PK__mlogic_TBsis_config
      primary key clustered,
    DFvalor varchar(8000) not null,
    DFdescricao nvarchar(4000) not null
      constraint DF__mlogic_TBsis_config_DFdescricao
      default ('')
  )
end
go
insert into mlogic.TBsis_config (DFchave, DFvalor, DFdescricao)
select DFchave, DFvalor, DFdescricao
  from (values
         ('replicacao.ativado', '0', 'Ativa (1) ou desativa (0) a replicação de dados do PDV para o Director.'),
         ('replicacao.historico.ativado', '0', 'Ativa (1) ou desativa (0) a gestão de histórico de replicação. Quando ativado apenas o número de dias determinado é mantido em histórico.'),
         ('replicacao.historico.dias', '30', 'Dias de histórico mantido nas tabelas de integração.')
       ) as t(DFchave, DFvalor, DFdescricao)
 where not exists (
         select 1
           from mlogic.TBsis_config
          where DFchave = t.DFchave
       )
go
-- select * from mlogic.TBsis_config
