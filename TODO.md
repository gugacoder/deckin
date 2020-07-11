#1 - TODO: Fix field name mapping

Today, field names are placed inside the "data" section.
They MUST be transfered to the "view" section.

From this:

    {
      data: {
        name: FIELDNAME,
        value: FIELDVALUE
      }
    }

To this:

    {
      data: {
        value: FIELDVALUE
      },
      view: {
        name: FIELDNAME
      }
    }
