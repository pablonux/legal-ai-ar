import React from 'react'
import Card from './Card';
import './card.scss';
import './card-grid.scss';

let patterns = [
    {
        link: '/content/pattern-detail',
        name: 'Card title here',
        prefixIconClass: 'Appkit4-icon icon-bar-chart-outline',
        dateTextIcon: 'Appkit4-icon icon-calendar-outline',
        dateText: 'Jan 19, 2023',
        timeTextIcon: 'Appkit4-icon icon-time-outline',
        timeText: '2 hours',
        description: 'Use the card component to preview a gallery of related items. This could be a collection of products or files.'
    },
    {
        link: '/content/pattern-detail',
        name: 'Card title here',
        prefixIconClass: 'Appkit4-icon icon-bar-chart-outline',
        dateTextIcon: 'Appkit4-icon icon-calendar-outline',
        dateText: 'Jan 19, 2023',
        timeTextIcon: 'Appkit4-icon icon-time-outline',
        timeText: '2 hours',
        description: 'Use the card component to preview a gallery of related items. This could be a collection of products or files.'
    },
    {
        link: '/content/pattern-detail',
        name: 'Card title here',
        prefixIconClass: 'Appkit4-icon icon-bar-chart-outline',
        dateTextIcon: 'Appkit4-icon icon-calendar-outline',
        dateText: 'Jan 19, 2023',
        timeTextIcon: 'Appkit4-icon icon-time-outline',
        timeText: '2 hours',
        description: 'Use the card component to preview a gallery of related items. This could be a collection of products or files.'
    },
    {
        link: '/content/pattern-detail',
        name: 'Card title here',
        prefixIconClass: 'Appkit4-icon icon-bar-chart-outline',
        dateTextIcon: 'Appkit4-icon icon-calendar-outline',
        dateText: 'Jan 19, 2023',
        timeTextIcon: 'Appkit4-icon icon-time-outline',
        timeText: '2 hours',
        description: 'Use the card component to preview a gallery of related items. This could be a collection of products or files.'
    }
];

let patternsWider = [
    {
        link: '/content/pattern-detail',
        name: 'Card title here',
        prefixIconClass: 'Appkit4-icon icon-bar-chart-outline',
        dateTextIcon: 'Appkit4-icon icon-calendar-outline',
        dateText: 'Jan 19, 2023',
        timeTextIcon: 'Appkit4-icon icon-time-outline',
        timeText: '2 hours',
        description: 'Use the card component to preview a gallery of related items. This could be a collection of products or files.'
    },
    {
        link: '/content/pattern-detail',
        name: 'Card title here',
        prefixIconClass: 'Appkit4-icon icon-bar-chart-outline',
        dateTextIcon: 'Appkit4-icon icon-calendar-outline',
        dateText: 'Jan 19, 2023',
        timeTextIcon: 'Appkit4-icon icon-time-outline',
        timeText: '2 hours',
        description: 'Use the card component to preview a gallery of related items. This could be a collection of products or files.'
    },
    {
        link: '/content/pattern-detail',
        name: 'Card title here',
        prefixIconClass: 'Appkit4-icon icon-bar-chart-outline',
        dateTextIcon: 'Appkit4-icon icon-calendar-outline',
        dateText: 'Jan 19, 2023',
        timeTextIcon: 'Appkit4-icon icon-time-outline',
        timeText: '2 hours',
        description: 'Use the card component to preview a gallery of related items. This could be a collection of products or files.'
    }
];

// let patterns = [
//   {
//     link: '/content/pattern-detail',
//     name: 'Card title here',
//     prefixIconClass: 'Appkit4-icon icon-bar-chart-outline',
//     dateTextIcon: 'Appkit4-icon icon-calendar-outline',
//     dateText: 'Jan 19, 2023',
//     timeTextIcon: 'Appkit4-icon icon-time-outline',
//     timeText: '2 hours',
//     src: require("./placeholder.svg").default,
//     description: 'Use the card component to preview a gallery of related items. This could be a collection of products or files.'
//   },
//   {
//     link: '/content/pattern-detail',
//     name: 'Card title here',
//     prefixIconClass: 'Appkit4-icon icon-bar-chart-outline',
//     dateTextIcon: 'Appkit4-icon icon-calendar-outline',
//     dateText: 'Jan 19, 2023',
//     timeTextIcon: 'Appkit4-icon icon-time-outline',
//     timeText: '2 hours',
//     src: require("./placeholder.svg").default,
//     description: 'Use the card component to preview a gallery of related items. This could be a collection of products or files.'
//   },
//   {
//     link: '/content/pattern-detail',
//     name: 'Card title here',
//     prefixIconClass: 'Appkit4-icon icon-bar-chart-outline',
//     dateTextIcon: 'Appkit4-icon icon-calendar-outline',
//     dateText: 'Jan 19, 2023',
//     timeTextIcon: 'Appkit4-icon icon-time-outline',
//     timeText: '2 hours',
//     src: require("./placeholder.svg").default,
//     description: 'Use the card component to preview a gallery of related items. This could be a collection of products or files.'
//   },
//   {
//     link: '/content/pattern-detail',
//     name: 'Card title here',
//     prefixIconClass: 'Appkit4-icon icon-bar-chart-outline',
//     dateTextIcon: 'Appkit4-icon icon-calendar-outline',
//     dateText: 'Jan 19, 2023',
//     timeTextIcon: 'Appkit4-icon icon-time-outline',
//     timeText: '2 hours',
//     src: require("./placeholder.svg").default,
//     description: 'Use the card component to preview a gallery of related items. This could be a collection of products or files.'
//   }
// ];

export default function CardGrid() {
    const onKeydown = (event: React.KeyboardEvent<HTMLElement>, url?: string) => {
        console.log(event.keyCode, url);
    }
    const onClick = (event: React.MouseEvent<HTMLElement>, url?: string) => {
        console.log(event, url);
    }
    //   return (
    //     <div className="ap-pattern-card-grid">
    //         {
    //             patterns.map((item, index: number) => {
    //             return <Card item={item} key={index} onKeydown={(event)=>onKeydown(event, item.link)}
    //             onClick={(event)=>onClick(event, item.link)}></Card>;
    //           })
    //         }
    //     </div>
    //   )

    return (
        <div className="card-grid-page">
            <div className="ap-pattern-card-grid-first">
                {
                    patterns.map((item, index: number) => {
                        return <Card className='first' item={item} key={index} onKeydown={(event) => onKeydown(event, item.link)}
                            onClick={(event) => onClick(event, item.link)}></Card>;
                    })
                }
            </div>
            <div className="ap-pattern-card-grid-second">
                {
                    patternsWider.map((item, index: number) => {
                        return <Card className='second' item={item} key={index} onKeydown={(event) => onKeydown(event, item.link)}
                            onClick={(event) => onClick(event, item.link)}></Card>;
                    })
                }
            </div>
        </div>
    )
}

