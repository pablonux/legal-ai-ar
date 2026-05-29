import React from 'react'
import SearchFilter from './SearchFilter';

export default function SearchFilterDefaultSolid() {

  const dataResult = {
    'Status': [
      { label: 'Opened issues', value: 1 },
      { label: 'Closed issues', value: 2 }
    ],
    'Framework': [
      { label: 'Angular', value: 1 },
      { label: 'React', value: 2 }
    ],
    'Issue type': [
    { label: 'General', value: 1 },
    { label: 'Bug', value: 2 },
    { label: 'Access', value: 3 },
    { label: 'Component', value: 4 },
    { label: 'Feature', value: 5 },
    { label: 'Documentation', value: 6 },
    { label: 'Design', value: 7 },
    { label: 'Installation', value: 8 }
  ]};

  return (
    <div style={{display: 'flex'}}>
      <SearchFilter solid data={dataResult} onSelect={(value, index)=>{console.log(value, index)}}></SearchFilter>
    </div>
  )
}
