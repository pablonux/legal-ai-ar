import { Filter, ItemDataType, Search, ValueType } from '@appkit4/react-components';
import React, { useEffect, useState } from 'react';
import classNames from 'classnames';
import './search-filter.scss';
const filterByProperty = (array: any[], prop: string, value: string) => {
    let filtered = [];
    for (let i = 0; i < array.length; i++) {
        let r = array[i];
        filtered.push(r.filter((item: any) => item[prop].toLowerCase().indexOf(value.toLowerCase()) > -1))
    }
    return filtered;
}
const convertObjectToArray = (originData: any) => {
    let dataListResult: ItemDataType[][] = [];
    originData && Object.keys(originData).forEach(item => {
        dataListResult.push(originData[item]);
    });
    return dataListResult;
}
export interface SearchFilterProps {
    title?: string;
    description?: string;
    data?: { [key: string]: ItemDataType[] };
    className?: string;
    solid?: boolean;
    onSelect?: (val: ValueType, index: number) => void;
    value?: ValueType[][];
    selectValueKey?: string;
}
const SearchFilter = React.forwardRef<HTMLElement, SearchFilterProps>((props: SearchFilterProps, ref) => {
    const {
        title = 'Task Filter',
        description = 'Use the Options Below to Filter the Task List for Quick Access.',
        data,
        className,
        solid,
        onSelect,
        value: valueProp,
        selectValueKey = "label"
    } = props;
    let dataList: ItemDataType[][] = [];
    dataList = convertObjectToArray(data);
    const [dataInfo, setDataInfo] = useState(dataList);

    useEffect(() => {
        dataList = convertObjectToArray(data)
        setDataInfo(dataList);
    }, [data])
    
    const handleChange = (value: string, event: React.SyntheticEvent) => {
        if (data) {
            const dataFilter = filterByProperty(dataList, selectValueKey, value);
            setDataInfo(dataFilter);
        }
    }
    const [value, setValue] = useState(valueProp||Array(dataInfo.length).fill([]));
    return (
        <div className={classNames('ap-pattern-search-filter', className, {
            'solid': solid
        })}>
            <div className='ap-pattern-search-filter-header'>
                <div className='search-filter-icon'>
                    <span className="Appkit4-icon icon-filter-outline"></span>
                </div>
                <div className="search-filter-description">
                    <div className='description-title'>{title}</div>
                    <div className='description-content'>{description}</div>
                </div>
                <div className='filter-header-hr'></div>
            </div>
            <Search
                searchType={"secondary"}
                placeholder='Search filters'
                onChange={handleChange}
            />
            <div className="search-menu">
                {
                    data && (Object.keys(data).length > 0) && (dataInfo && dataInfo.flat().length > 0 ? dataInfo?.map(((dataItem: ItemDataType[], index) => {
                        
                        if (dataItem.length > 0)
                            return <Filter
                                key={index}
                                title={Object.keys(data!)?.[index]}
                                multiple={true}
                                data={dataItem}
                                valueKey={selectValueKey}
                                expand={true}
                                value={value[index]}
                                onSelect={(val) => {
                                    setValue((value: any[]) => value.map((item: string[], curIndex: number) => {
                                        if (curIndex === index) {
                                            return val;
                                        } else {
                                            return item;
                                        }
                                    }));
                                    onSelect?.(val, index);
                                }}
                            >
                            </Filter>
                    })) : <div className='search-text'>Nothing matches your results</div>)
                }
            </div>
        </div>
    )
})

export default SearchFilter

