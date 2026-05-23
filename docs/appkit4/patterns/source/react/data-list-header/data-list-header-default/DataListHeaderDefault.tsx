import { Button } from '@appkit4/react-components';
import DataListHeader from './DataListHeader';

export default function DataListHeaderDefault() {
  return (
    <div style={{transform: 'translateY(100%)'}}>
      <DataListHeader
        selectedStr='Selected 2 of 250'
        handleSeachChange={(value: string) => { console.log(value) }}
        filterList={[
          { id: "0", primary: "Manager", secondary: "27 items" },
          { id: "1", primary: "Client", secondary: "34 items" },
          { id: "2", primary: "PwC", secondary: "19 items" }
        ]}
        renderIcons={<>
          <span tabIndex={0} className="Appkit4-icon icon-person-outline" role="button" aria-label="person" onClick={() => console.log('person')}></span>
          <span tabIndex={0} className="Appkit4-icon icon-lockopen-unlock-outline" role="button" aria-label="unlock" onClick={() => console.log('unlock')}></span>
          <span tabIndex={0} className="Appkit4-icon icon-redo-outline" role="button" aria-label="redo" onClick={() => console.log('redo')}></span>
          <span tabIndex={0} className="Appkit4-icon icon-delete-outline" role="button" aria-label="delete" onClick={() => console.log('delete')}></span>
        </>}
      ></DataListHeader>
    </div>
  )
}
