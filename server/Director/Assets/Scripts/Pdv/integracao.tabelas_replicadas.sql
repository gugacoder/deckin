create or replace view integracao.tabelas_replicadas
as
select table_name as tabela
  from information_schema.tables
 where table_schema='integracao'
   and table_type = 'BASE TABLE'
;

