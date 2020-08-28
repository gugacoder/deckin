if object_id('mlogic.vw_pdv') is not null
  drop view mlogic.vw_pdv
GO
create view mlogic.vw_pdv
as
select 1 as DFid_pdv
     , 1 as DFcod_empresa
     , 'PDV001' as DFdescricao
     , 'stormwind' as DFendereco_rede
     , 1 as DFativado
     , (select cast(DFvalor as int)
          from mlogic.TBsis_config
         where DFchave = 'replicacao.ativado'
       ) as DFreplicacao_ativado
GO
-- select * from mlogic.vw_pdv

